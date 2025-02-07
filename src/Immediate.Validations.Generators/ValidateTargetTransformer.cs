using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
		_members = ((INamedTypeSymbol)context.TargetSymbol)
			.GetAllMembers()
			.Where(m =>
				m is IPropertySymbol or IFieldSymbol
					or IMethodSymbol
				{
					Parameters: [],
					MethodKind: MethodKind.Ordinary,
				}
			)
			.ToList();

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
			.Select(na => na.Value.Value is bool b && b)
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
					property.GetAttributes()
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
		ImmutableArray<AttributeData> attributes
	)
	{
		_token.ThrowIfCancellationRequested();

		var isReferenceType = propertyType.IsReferenceType;
		var isNullable = isReferenceType
			? (nullableAnnotation is NullableAnnotation.Annotated
				|| attributes.Any(a => a.AttributeClass.IsAllowNullAttribute()))
			: propertyType.IsNullableType();

		var validateNotNull = !isNullable || attributes.Any(a => a.AttributeClass.IsNotNullAttribute());
		var validateMessage = attributes
			.FirstOrDefault(a => a.AttributeClass.IsNotNullAttribute())
			?.NamedArguments
			.FirstOrDefault(a => a.Key is "Message") is { Value: { Value: { } } value }
			? value.ToCSharpString()
			: null;

		var baseType = propertyType.IsNullableType()
			? ((INamedTypeSymbol)propertyType).TypeArguments[0]
			: propertyType;

		_token.ThrowIfCancellationRequested();

		var isValidationProperty = propertyType.GetAttributes()
			.Any(v => v.AttributeClass.IsValidateAttribute());

		_token.ThrowIfCancellationRequested();

		var validations = new List<PropertyValidation>();

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

		foreach (var attribute in attributes)
		{
			if (ProcessAttribute(
					propertyType,
					baseType,
					attribute
				) is { } validation
			)
			{
				validations.Add(validation);
			}
		}

		var collectionPropertyDetails = propertyType switch
		{
			IArrayTypeSymbol ats =>
				GetPropertyValidations(
					name,
					propertyName,
					ats.ElementType,
					ats.ElementNullableAnnotation,
					attributes
				),

			INamedTypeSymbol
			{
				IsGenericType: true,
				TypeArguments: [{ } type],
				TypeArgumentNullableAnnotations: [{ } annotation],
			} nts when nts.AllInterfaces.Any(i => i.IsICollection1() || i.IsIReadOnlyCollection1()) =>
				GetPropertyValidations(
					name,
					propertyName,
					type,
					annotation,
					attributes
				),

			_ => null,
		};

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

	private PropertyValidation? ProcessAttribute(
		ITypeSymbol propertyType,
		ITypeSymbol baseType,
		AttributeData attribute
	)
	{
		_token.ThrowIfCancellationRequested();

		var @class = attribute.AttributeClass;
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
		);

		_token.ThrowIfCancellationRequested();

		return new()
		{
			ValidatorName = @class.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
			IsGenericMethod = validateMethod.IsGenericMethod,
			IsNullable = targetParameterType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated }
				or { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T },
			Arguments = parameters.ToEquatableReadOnlyList()!,
			Message = GetMessage(attribute),
		};
	}

	private static string? GetMessage(AttributeData attribute)
	{
		foreach (var p in attribute.NamedArguments)
		{
			if (p is { Key: "Message", Value.Value: string s })
				return $"\"{s}\"";
		}

		return null;
	}

	private List<Argument>? BuildArgumentValues(
		AttributeData attribute,
		ImmutableArray<IParameterSymbol> parameters,
		ITypeSymbol propertyType
	)
	{
		var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference!.GetSyntax();
		var argumentListSyntax = attributeSyntax.ArgumentList?.Arguments ?? [];

		if (argumentListSyntax.Count == 0)
			return null;

		if (attribute.AttributeConstructor is null)
			return null;

		var attributeParameters = attribute.AttributeConstructor.Parameters;
		List<IPropertySymbol>? attributeProperties = null;

		var argumentValues = new List<Argument>(argumentListSyntax.Count);

		for (var i = 0; i < argumentListSyntax.Count; i++)
		{
			_token.ThrowIfCancellationRequested();

			switch (argumentListSyntax[i])
			{
				// Message = "Value"
				case { NameEquals.Name.Identifier.ValueText: var name } syntax:
				{
					if (name is "Message")
						break;

					attributeProperties ??= attribute.AttributeClass!.GetMembers()
						.OfType<IPropertySymbol>()
						.ToList();
					var property = attributeProperties
						.First(a => string.Equals(a.Name, name, StringComparison.Ordinal));

					var parameterValue = BuildArgumentValue(
						syntax,
						property,
						parameters
					);
					argumentValues.Add(parameterValue);

					break;
				}

				// operand: "Value"
				case { NameColon.Name.Identifier.ValueText: var name, Expression: { } expr } syntax:
				{
					for (var j = 0; j < attributeParameters.Length; j++)
					{
						if (string.Equals(attributeParameters[j].Name, name, StringComparison.Ordinal))
						{
							var parameterValue = BuildArgumentValue(
								syntax,
								attributeParameters[j],
								parameters
							);

							argumentValues.Add(parameterValue);
							break;
						}
					}

					break;
				}

				// "Value"
				case var syntax:
				{
					var attributeParameter = attributeParameters[i];
					if (!attributeParameter.IsParams)
					{
						argumentValues.Add(
							BuildArgumentValue(
								syntax,
								attributeParameter,
								parameters
							)
						);

						break;
					}

					var parameter = GetParameter(attributeParameter.Name, parameters);
					if (parameter.Type is not IArrayTypeSymbol { ElementType: { } elementType })
						break;

					if (elementType is ITypeParameterSymbol tps)
						elementType = propertyType;

					var (argumentName, argumentValue, isArray) = GetArgumentValue(
						attributeParameter,
						syntax
					);

					if (isArray)
					{
						argumentValues.Add(
							BuildArgumentValue(
								attributeParameter,
								parameters,
								argumentName,
								argumentValue,
								arrayType: elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
							)
						);

						break;
					}

					var values = new List<string>()
					{
						argumentValue,
					};

					for (i++; i < argumentListSyntax.Count; i++)
					{
						if (argumentListSyntax[i] is { NameEquals: not null })
						{
							i--;
							break;
						}

						(_, argumentValue, _) = GetArgumentValue(
							attributeParameter,
							argumentListSyntax[i]
						);

						values.Add(argumentValue);
					}

					argumentValues.Add(
						BuildArgumentValue(
							attributeParameter,
							parameters,
							"",
							$"[{string.Join(", ", values)}]",
							arrayType: elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
						)
					);

					break;
				}
			}
		}

		return argumentValues;
	}

	private Argument BuildArgumentValue(
		AttributeArgumentSyntax attributeArgumentSyntax,
		ISymbol parameterSymbol,
		ImmutableArray<IParameterSymbol> parameters
	)
	{
		var (argumentName, argumentValue, _) = GetArgumentValue(parameterSymbol, attributeArgumentSyntax);

		return BuildArgumentValue(
			parameterSymbol,
			parameters,
			argumentName,
			argumentValue,
			arrayType: null
		);
	}

	private static Argument BuildArgumentValue(
		ISymbol parameterSymbol,
		ImmutableArray<IParameterSymbol> parameters,
		string argumentName,
		string argumentValue,
		string? arrayType
	)
	{
		var parameterName = GetParameterName(parameterSymbol.Name, parameters);

		return new()
		{
			ParameterName = parameterName.ToPascalCase(),
			NamedParameterName = $"{parameterName}: ",
			Name = argumentName,
			Value = argumentValue,
			ArrayType = arrayType,
		};
	}

	private static string GetParameterName(string name, ImmutableArray<IParameterSymbol> parameters) =>
		GetParameter(name, parameters).Name;

	private static IParameterSymbol GetParameter(string name, ImmutableArray<IParameterSymbol> parameters)
	{
		foreach (var p in parameters)
		{
			if (p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				return p;
		}

		throw new InvalidOperationException();
	}

	private (string Name, string Value, bool IsArray) GetArgumentValue(
		ISymbol parameterSymbol,
		AttributeArgumentSyntax attributeArgumentSyntax
	)
	{
		if (parameterSymbol.IsTargetTypeSymbol()
			&& attributeArgumentSyntax.Expression.IsNameOfExpression(out var argumentExpression))
		{
			if (argumentExpression is SimpleNameSyntax { Identifier.ValueText: { } name })
			{
				var member = _members.FirstOrDefault(m => m.Name.Equals(name, StringComparison.Ordinal));

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
				var symbol = _semanticModel.GetSymbolInfo(argumentExpression);

				return (
					"",
					symbol.Symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "",
					false
				);
			}
		}
		else
		{
			var operation = _semanticModel
				.GetOperation(attributeArgumentSyntax.Expression);

			if (operation?.Type is INamedTypeSymbol { TypeKind: TypeKind.Enum })
			{
				var symbolInfo = _semanticModel.GetSymbolInfo(attributeArgumentSyntax.Expression);
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
	public static bool IsTargetTypeSymbol(this ISymbol symbol) =>
		symbol switch
		{
			IParameterSymbol { Type: var type } => type.IsValidTargetTypeType(),
			IPropertySymbol { Type: var type } => type.IsValidTargetTypeType(),
			_ => false,
		}
		&& symbol.GetAttributes().Any(a => a.AttributeClass.IsTargetTypeAttribute());

	private static bool IsValidTargetTypeType(this ITypeSymbol? typeSymbol) =>
		typeSymbol is { SpecialType: SpecialType.System_Object or SpecialType.System_String }
			or IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Object or SpecialType.System_String };

	public static bool IsNameOfExpression(this ExpressionSyntax syntax, [NotNullWhen(returnValue: true)] out ExpressionSyntax? argumentExpression)
	{
		if (syntax is InvocationExpressionSyntax
			{
				Expression: SimpleNameSyntax { Identifier.ValueText: "nameof" },
				ArgumentList.Arguments: [{ Expression: { } expr }],
			}
		)
		{
			argumentExpression = expr;
			return true;
		}
		else
		{
			argumentExpression = null;
			return false;
		}
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
