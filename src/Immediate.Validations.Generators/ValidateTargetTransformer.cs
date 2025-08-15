using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Immediate.Validations.Generators;

public sealed class ValidateTargetTransformer
{
	private static readonly SymbolDisplayFormat s_fullyQualifiedPlusNullable =
		SymbolDisplayFormat.FullyQualifiedFormat
			.WithMiscellaneousOptions(
				SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
			);

	private readonly GeneratorAttributeSyntaxContext _context;
	private readonly CancellationToken _token;
	private readonly SemanticModel _semanticModel;
	private readonly INamedTypeSymbol _symbol;
	private readonly List<ISymbol> _members;

	public ValidateTargetTransformer(
		GeneratorAttributeSyntaxContext context,
		CancellationToken token
	)
	{
		_context = context;
		_token = token;
		_semanticModel = _context.SemanticModel;
		_symbol = (INamedTypeSymbol)context.TargetSymbol;

		_token.ThrowIfCancellationRequested();
		_members =
		[
			.. ((INamedTypeSymbol)context.TargetSymbol)
				.GetAllMembers()
				.Where(m =>
					m is IPropertySymbol or IFieldSymbol
						or IMethodSymbol
					{
						Parameters: [],
						MethodKind: MethodKind.Ordinary,
					}
				),
		];

		_token.ThrowIfCancellationRequested();
	}

	public ValidationTarget? Transform()
	{
		_token.ThrowIfCancellationRequested();

		var @namespace = _symbol.ContainingNamespace.ToString().NullIf("<global namespace>");
		var outerClasses = GetOuterClasses();
		var @class = GetClass(_symbol);
		var hasAdditionalValidationsMethod = _symbol.HasAdditionalValidationsMethod();
		var baseValidationTargets = GetBaseValidationTargets();
		var skipSelf = GetSkipSelf();
		var properties = skipSelf ? [] : GetProperties();

		return new()
		{
			Namespace = @namespace,
			OuterClasses = outerClasses,
			Class = @class,
			HasAdditionalValidationsMethod = hasAdditionalValidationsMethod,
			IsReferenceType = _symbol.IsReferenceType,
			SkipSelf = skipSelf,
			BaseValidationTargets = baseValidationTargets,
			Properties = properties,
		};
	}

	private static Class GetClass(INamedTypeSymbol symbol) =>
		new()
		{
			Name = symbol.Name,
			Type = symbol switch
			{
				{ TypeKind: TypeKind.Interface } => "interface",
				{ IsRecord: true, TypeKind: TypeKind.Struct, } => "record struct",
				{ IsRecord: true, } => "record",
				{ TypeKind: TypeKind.Struct, } => "struct",
				_ => "class",
			},
		};

	private EquatableReadOnlyList<Class> GetOuterClasses()
	{
		List<Class>? outerClasses = null;
		var outerSymbol = _symbol.ContainingType;
		while (outerSymbol is not null)
		{
			(outerClasses ??= []).Add(GetClass(outerSymbol));
			outerSymbol = outerSymbol.ContainingType;
		}

		if (outerClasses is null)
			return default;

		outerClasses.Reverse();

		return outerClasses.ToEquatableReadOnlyList();
	}

	private EquatableReadOnlyList<string> GetBaseValidationTargets()
	{
		List<string>? baseValidationTargets = null;

		if (_symbol.BaseType.IsValidationTarget())
			(baseValidationTargets = []).Add(_symbol.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

		foreach (var i in _symbol.Interfaces)
		{
			if (i.IsValidationTarget())
				(baseValidationTargets ??= []).Add(i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
		}

		if (baseValidationTargets is null)
			return default;

		return baseValidationTargets.ToEquatableReadOnlyList();
	}

	private bool GetSkipSelf()
	{
		var attribute = _context.Attributes[0];

		var skipSelf = attribute.NamedArguments
			.Where(na => na.Key is "SkipSelf")
			.Select(na => na.Value.Value is true)
			.FirstOrDefault();

		return skipSelf;
	}

	private EquatableReadOnlyList<ValidationTargetProperty> GetProperties()
	{
		_token.ThrowIfCancellationRequested();

		var properties = new List<ValidationTargetProperty>();
		foreach (var member in _symbol.GetMembers())
		{
			if (member is not IPropertySymbol
				{
					DeclaredAccessibility: Accessibility.Public,
					IsStatic: false,
					// ignore record `EqualityContract`
					Name: not "EqualityContract",
				} property
			)
			{
				continue;
			}

			if (_symbol.TypeKind is not TypeKind.Interface && property.SetMethod is null)
				continue;

			_token.ThrowIfCancellationRequested();

			if (GetPropertyValidations(
					property.GetDescription(),
					property.Name,
					property.Type,
					property.NullableAnnotation,
					property.GetTargetedAttributes(_semanticModel)
				) is { } prop)
			{
				properties.Add(prop);
			}
		}

		return properties.ToEquatableReadOnlyList();
	}

	private ValidationTargetProperty? GetPropertyValidations(
		string name,
		string propertyName,
		ITypeSymbol propertyType,
		NullableAnnotation nullableAnnotation,
		IReadOnlyList<(string? Target, IObjectCreationOperation AttributeOperation)> targettedAttributes,
		bool isCollectionElement = false
	)
	{
		_token.ThrowIfCancellationRequested();

		var (collectionPropertyDetails, attributes) = propertyType switch
		{
			IArrayTypeSymbol ats =>
				(
					GetPropertyValidations(
						name,
						propertyName,
						ats.ElementType,
						ats.ElementNullableAnnotation,
						targettedAttributes,
						isCollectionElement: true
					),
					targettedAttributes.Where(a => a.Target is null).Select(a => a.AttributeOperation)
				),

			INamedTypeSymbol
			{
				IsGenericType: true,
				TypeArguments: [{ } type],
				TypeArgumentNullableAnnotations: [{ } annotation],
			} nts when
				nts.IsICollection1()
				|| nts.IsIReadOnlyCollection1()
				|| nts.AllInterfaces.Any(i => i.IsICollection1() || i.IsIReadOnlyCollection1()) =>
				(
					GetPropertyValidations(
						name,
						propertyName,
						type,
						annotation,
						targettedAttributes,
						isCollectionElement: true
					),
					targettedAttributes.Where(a => a.Target is null).Select(a => a.AttributeOperation)
				),

			_ when !isCollectionElement =>
				(
					null,
					targettedAttributes.Where(a => a.Target is null).Select(a => a.AttributeOperation)
				),

			_ =>
				(
					null,
					targettedAttributes.Where(a => a.Target is "element").Select(a => a.AttributeOperation)
				),
		};

		_token.ThrowIfCancellationRequested();

		var isReferenceType = propertyType.IsReferenceType;
		var isNullable = isReferenceType
			? (nullableAnnotation is NullableAnnotation.Annotated
				|| attributes.Any(a => a.Type.IsAllowNullAttribute()))
			: propertyType.IsNullableType();

		var isNotNullAttribute = attributes.FirstOrDefault(a => a.Type.IsNotNullAttribute());
		var validateNotNull = !isNullable || isNotNullAttribute is { };
		var validateMessage = isNotNullAttribute?.GetMessage();

		var baseType = propertyType.IsNullableType()
			? ((INamedTypeSymbol)propertyType).TypeArguments[0]
			: propertyType;

		_token.ThrowIfCancellationRequested();

		var isValidationProperty = propertyType.GetAttributes()
			.Any(v => v.AttributeClass.IsValidateAttribute());

		_token.ThrowIfCancellationRequested();

		var validations = ProcessAttributes(
			propertyType,
			baseType,
			attributes
		);

		if (baseType.TypeKind is TypeKind.Enum)
		{
			validations.Add(
				new()
				{
					ValidatorName = "global::Immediate.Validations.Shared.EnumValueAttribute",
					IsGenericMethod = true,
					IsNullable = false,
					Arguments = [],
					Message = null,
				}
			);
		}

		_token.ThrowIfCancellationRequested();

		if (
			(!validateNotNull
				|| (!isReferenceType && !propertyType.IsNullableType()))
			&& !isValidationProperty
			&& collectionPropertyDetails is null
			&& validations is []
		)
		{
			return null;
		}

		return new()
		{
			Name = name,
			PropertyName = propertyName,
			TypeFullName = propertyType.ToDisplayString(s_fullyQualifiedPlusNullable),
			IsReferenceType = isReferenceType,
			IsNullable = isNullable,

			ValidateNotNull = validateNotNull
				? new()
				{
					Message = validateMessage,
					IsNullable = true,
					Arguments = [],
					IsGenericMethod = false,
					ValidatorName = "global::Immediate.Validations.Shared.NotNullAttribute",
				}
				: null,

			IsValidationProperty = isValidationProperty,
			ValidationTypeFullName = isValidationProperty
				? baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				: null,

			CollectionPropertyDetails = collectionPropertyDetails,

			Validations = validations
				.Where(v => !v.IsNullable)
				.ToEquatableReadOnlyList(),
			NullValidations = validations
				.Where(v => v.IsNullable)
				.ToEquatableReadOnlyList(),
		};
	}

	private List<PropertyValidation> ProcessAttributes(
		ITypeSymbol propertyType,
		ITypeSymbol baseType,
		IEnumerable<IObjectCreationOperation> attributes
	)
	{
		var list = new List<PropertyValidation>();

		foreach (var attribute in attributes)
		{
			if (ProcessAttribute(
					propertyType,
					baseType,
					attribute
				) is { } validation
			)
			{
				list.Add(validation);
			}
		}

		return list;
	}

	private PropertyValidation? ProcessAttribute(
		ITypeSymbol propertyType,
		ITypeSymbol baseType,
		IObjectCreationOperation attribute
	)
	{
		_token.ThrowIfCancellationRequested();

		var @class = (INamedTypeSymbol?)attribute.Type;
		if (!@class.ImplementsValidatorAttribute() || @class.IsNotNullAttribute())
			return null;

		_token.ThrowIfCancellationRequested();

		if (@class
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.IsValidValidatePropertyMethod())
				.SingleValue() is not
				{
					Parameters: [{ Type: { } targetParameterType }, ..],
				} validateMethod
		)
		{
			return null;
		}

		_token.ThrowIfCancellationRequested();

		if (targetParameterType is ITypeParameterSymbol tps)
		{
			if (!tps.SatisfiesConstraints(propertyType, _semanticModel.Compilation))
				return null;
		}
		else
		{
			var conversion = _semanticModel.Compilation
				.ClassifyConversion(baseType, targetParameterType);

			if (!conversion.IsValidConversion())
				return null;
		}

		_token.ThrowIfCancellationRequested();

		var parameters = BuildArgumentValues(
			attribute,
			validateMethod.Parameters,
			propertyType
		) ?? [];

		_token.ThrowIfCancellationRequested();

		return new()
		{
			ValidatorName = @class.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
			IsGenericMethod = validateMethod.IsGenericMethod,
			IsNullable = targetParameterType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated }
				or { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T },
			Arguments = parameters.ToEquatableReadOnlyList()!,
			Message = attribute.GetMessage(),
		};
	}

	private List<Argument>? BuildArgumentValues(
		IObjectCreationOperation attribute,
		ImmutableArray<IParameterSymbol> validateMethodParameters,
		ITypeSymbol targetPropertyType
	)
	{
		if (attribute.Constructor is null)
			return null;

		var argumentValues = new List<Argument>(
			attribute.Arguments.Length
			+ (attribute.Initializer?.Initializers.Length ?? 0)
		);

		foreach (var argument in attribute.Arguments)
		{
			_token.ThrowIfCancellationRequested();

			var constructorParameter = argument.Parameter!;
			var validateMethodParameter = GetParameter(constructorParameter.Name, validateMethodParameters);

			if (validateMethodParameter.Type is not IArrayTypeSymbol { ElementType: { } elementType })
			{
				if (argument.Syntax is AttributeArgumentSyntax aas)
				{
					argumentValues.Add(
						BuildArgumentValue(
							aas,
							constructorParameter,
							validateMethodParameters
						)
					);
				}

				continue;
			}

			if (argument.Value is not IArrayCreationOperation
				{
					Initializer.ElementValues: { } elements
				})
			{
				return null;
			}

			if (elementType is ITypeParameterSymbol tps)
				elementType = targetPropertyType;

			if (elements.Length == 1
				&& GetArgumentValue(
					constructorParameter,
					(ExpressionSyntax)elements[0].Syntax
				) is (var name, var value, true))
			{
				argumentValues.Add(
					BuildArgumentValue(
						constructorParameter,
						validateMethodParameters,
						name,
						value,
						arrayType: elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
					)
				);

				continue;
			}

			var elementValues = elements
				.Select(e =>
					GetArgumentValue(
						constructorParameter,
						(ExpressionSyntax)e.Syntax
					).Value
				)
				.ToList();

			argumentValues.Add(
				BuildArgumentValue(
					constructorParameter,
					validateMethodParameters,
					"",
					$"[{string.Join(", ", elementValues)}]",
					arrayType: elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				)
			);
		}

		if (attribute.Initializer?.Initializers is { } initializers)
		{
			foreach (var initializer in initializers)
			{
				_token.ThrowIfCancellationRequested();

				if (initializer is not ISimpleAssignmentOperation
					{
						Target: IPropertyReferenceOperation { Property: { } property }
					})
				{
					return null;
				}

				if (property.Name is "Message")
					continue;

				argumentValues.Add(
					BuildArgumentValue(
						(AttributeArgumentSyntax)initializer.Syntax,
						GetParameter(property.Name, validateMethodParameters),
						validateMethodParameters
					)
				);
			}
		}

		return argumentValues;
	}

	private Argument BuildArgumentValue(
		AttributeArgumentSyntax attributeArgumentSyntax,
		ISymbol constructorParameterSymbol,
		ImmutableArray<IParameterSymbol> parameters
	)
	{
		var (argumentName, argumentValue, _) = GetArgumentValue(
			constructorParameterSymbol,
			attributeArgumentSyntax.Expression
		);

		return BuildArgumentValue(
			constructorParameterSymbol,
			parameters,
			argumentName,
			argumentValue,
			arrayType: null
		);
	}

	private static Argument BuildArgumentValue(
		ISymbol constructorParameterSymbol,
		ImmutableArray<IParameterSymbol> parameters,
		string argumentName,
		string argumentValue,
		string? arrayType
	)
	{
		var parameterName = GetParameter(constructorParameterSymbol.Name, parameters).Name;

		return new()
		{
			ParameterName = parameterName.ToPascalCase(),
			NamedParameterName = $"{parameterName}: ",
			Name = argumentName,
			Value = argumentValue,
			ArrayType = arrayType,
		};
	}

	private static IParameterSymbol GetParameter(string name, ImmutableArray<IParameterSymbol> validateMethodParameters)
	{
		foreach (var p in validateMethodParameters)
		{
			if (p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				return p;
		}

		throw new InvalidOperationException();
	}

	private (string Name, string Value, bool IsArray) GetArgumentValue(
		ISymbol constructorParameterSymbol,
		ExpressionSyntax argumentExpressionSyntax
	)
	{
		if (constructorParameterSymbol.IsTargetTypeSymbol()
			&& argumentExpressionSyntax.IsNameOfExpression(out var argumentExpression))
		{
			if (argumentExpression is SimpleNameSyntax { Identifier.ValueText: { } name })
			{
				var member = _members.Find(m => m.Name.Equals(name, StringComparison.Ordinal));

				return (
					member.GetDescription(),
					member switch
					{
						IMethodSymbol { IsStatic: true } => $"{name}()",
						IMethodSymbol => $"instance.{name}()",
						{ IsStatic: true } => $"{name}",
						_ => $"instance.{name}",
					},
					member switch
					{
						IMethodSymbol { ReturnType: var type } => type,
						IFieldSymbol { Type: var type } => type,
						IPropertySymbol { Type: var type } => type,
						_ => null,
					} is IArrayTypeSymbol
				);
			}
			else
			{
				var symbolInfo = _semanticModel.GetSymbolInfo(argumentExpression, _token);

				var symbol = symbolInfo.Symbol
					?? symbolInfo.CandidateSymbols
						.FirstOrDefault(
							ims => ims is IMethodSymbol
							{
								Parameters: []
							}
						);

				return (
					argumentExpression.ToString().Replace(".", "").ToTitleCase() ?? "",
					symbol?.ToDisplayString(DisplayNameFormatters.FullyQualifiedForMembers) ?? "",
					false
				);
			}
		}
		else
		{
			var operation = _semanticModel.GetOperation(argumentExpressionSyntax, _token);

			if (operation?.Type is INamedTypeSymbol { TypeKind: TypeKind.Enum })
			{
				var symbolInfo = _semanticModel.GetSymbolInfo(argumentExpressionSyntax);
				var symbol = (IFieldSymbol)symbolInfo.Symbol!;
				var reference = $"{symbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{symbol.Name}";

				return ("", reference, false);
			}

			return (
				"",
				operation?.ConstantValue switch
				{
					{ HasValue: true, Value: string s } =>
						SymbolDisplay.FormatLiteral(s, quote: true),

					{ HasValue: true, Value: { } o } =>
						SymbolDisplay.FormatPrimitive(o, quoteStrings: false, useHexadecimalNumbers: false),

					_ => "",
				},
				false
			);
		}
	}
}

file static class Extensions
{
	public static string? GetMessage(this IObjectCreationOperation attribute)
	{
		return attribute
			?.Initializer
			?.Initializers
			.OfType<ISimpleAssignmentOperation>()
			.FirstOrDefault(
				a =>
					string.Equals(
						((IPropertyReferenceOperation)a.Target).Property.Name,
						"Message",
						StringComparison.Ordinal
					)
			)
			?.Value
			.ConstantValue is { HasValue: true, Value: string s }
			? SymbolDisplay.FormatLiteral(s, quote: true)
			: null;
	}

	public static string GetDescription(this ISymbol symbol)
	{
		if (symbol is IMethodSymbol)
			return $"{symbol.Name}()";

		if (symbol is IFieldSymbol or IPropertySymbol)
		{
			foreach (var attribute in symbol.GetAttributes())
			{
				if (attribute.AttributeClass.IsDescriptionAttribute()
					&& attribute.ConstructorArguments is [{ Value: string v }]
					&& !string.IsNullOrWhiteSpace(v))
				{
					return v;
				}
			}
		}

		return symbol.Name.ToTitleCase();
	}
}
