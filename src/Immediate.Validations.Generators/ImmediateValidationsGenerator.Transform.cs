using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Validations.Generators;

public sealed partial class ImmediateValidationsGenerator
{
	private static readonly SymbolDisplayFormat s_fullyQualifiedPlusNullable =
		SymbolDisplayFormat.FullyQualifiedFormat
			.WithMiscellaneousOptions(
				SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
			);

	private static ValidationTarget? TransformMethod(
		GeneratorAttributeSyntaxContext context,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var symbol = (INamedTypeSymbol)context.TargetSymbol;
		var @namespace = symbol.ContainingNamespace.ToString().NullIf("<global namespace>");
		var outerClasses = GetOuterClasses(symbol);
		var baseValidatorTypes = GetBaseValidatorTypes(symbol);
		var properties = GetProperties(context.SemanticModel.Compilation, symbol, token);

		return new()
		{
			Namespace = @namespace,
			OuterClasses = outerClasses,
			Class = GetClass(symbol),
			HasAdditionalValidationsMethod = symbol.HasAdditionalValidationsMethod(),
			IsReferenceType = symbol.IsReferenceType,
			BaseValidatorTypes = baseValidatorTypes,
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

	private static EquatableReadOnlyList<Class> GetOuterClasses(INamedTypeSymbol symbol)
	{
		List<Class>? outerClasses = null;
		var outerSymbol = symbol.ContainingType;
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

	private static EquatableReadOnlyList<string> GetBaseValidatorTypes(INamedTypeSymbol symbol)
	{
		List<string>? baseValidatorTypes = null;

		if (symbol.BaseType.IsValidationTarget())
			(baseValidatorTypes = []).Add(symbol.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

		foreach (var i in symbol.Interfaces)
		{
			if (i.IsValidationTarget())
				(baseValidatorTypes ??= []).Add(i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
		}

		if (baseValidatorTypes is null)
			return default;

		return baseValidatorTypes.ToEquatableReadOnlyList();
	}

	private static EquatableReadOnlyList<ValidationTargetProperty> GetProperties(
		Compilation compilation,
		INamedTypeSymbol symbol,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var properties = new List<ValidationTargetProperty>();
		foreach (var member in symbol.GetMembers())
		{
			if (member is not IPropertySymbol
				{
					IsStatic: false,
					// ignore record `EqualityContract`
					Name: not "EqualityContract",
				} property
			)
			{
				continue;
			}

			token.ThrowIfCancellationRequested();

			if (GetPropertyValidations(
					compilation,
					property.Name,
					property.Type,
					property.NullableAnnotation,
					property.GetAttributes(),
					token
				) is { } prop)
			{
				properties.Add(prop);
			}
		}

		return properties.ToEquatableReadOnlyList();
	}

	private static ValidationTargetProperty? GetPropertyValidations(
		Compilation compilation,
		string propertyName,
		ITypeSymbol propertyType,
		NullableAnnotation nullableAnnotation,
		ImmutableArray<AttributeData> attributes,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var isReferenceType = propertyType.IsReferenceType;
		var isNullable = isReferenceType
			? nullableAnnotation is NullableAnnotation.Annotated
			: propertyType.IsNullableType();

		var baseType = !isReferenceType && isNullable
			? ((INamedTypeSymbol)propertyType).TypeArguments[0]
			: propertyType;

		token.ThrowIfCancellationRequested();

		var isValidationProperty = propertyType.GetAttributes()
			.Any(v => v.AttributeClass.IsValidateAttribute());

		token.ThrowIfCancellationRequested();

		var validations = new List<PropertyValidation>();

		if (baseType.TypeKind is TypeKind.Enum)
		{
			validations.Add(
				new()
				{
					ValidatorName = "global::Immediate.Validations.Shared.EnumValueAttribute",
					IsGenericMethod = true,
					IsNullable = false,
					Parameters = [],
					Message = null,
				}
			);
		}

		token.ThrowIfCancellationRequested();

		foreach (var attribute in attributes)
		{
			token.ThrowIfCancellationRequested();

			var @class = attribute.AttributeClass?.OriginalDefinition;
			if (!@class.ImplementsValidatorAttribute())
				continue;

			token.ThrowIfCancellationRequested();

			if (@class
					.GetMembers()
					.OfType<IMethodSymbol>()
					.Where(m => m is
					{
						IsStatic: true,
						Parameters.Length: >= 1,
						Name: "ValidateProperty",
						ReturnType: INamedTypeSymbol
						{
							MetadataName: "ValueTuple`2",
							ContainingNamespace:
							{
								Name: "System",
								ContainingNamespace.IsGlobalNamespace: true,
							},
							TypeArguments:
							[
							{ SpecialType: SpecialType.System_Boolean },
							{ SpecialType: SpecialType.System_String },
							]
						},
					})
					.SingleValue() is not
					{
						Parameters: [{ Type: { } targetParameterType }, ..],
					} validateMethod
			)
			{
				continue;
			}

			token.ThrowIfCancellationRequested();

			if (targetParameterType is ITypeParameterSymbol tps)
			{
				if (!Utility.SatisfiesConstraints(tps, propertyType, compilation))
					continue;
			}
			else
			{
				var conversion = compilation.ClassifyConversion(baseType, targetParameterType);
				if (conversion is not { IsIdentity: true }
						or { IsImplicit: true, IsReference: true }
						or { IsImplicit: true, IsNullable: true }
						or { IsBoxing: true }
				)
				{
					continue;
				}
			}

			token.ThrowIfCancellationRequested();

			var parameters = BuildParameterValues(attribute, validateMethod.Parameters);

			token.ThrowIfCancellationRequested();

			validations.Add(
				new()
				{
					ValidatorName = @class.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
					IsGenericMethod = validateMethod.IsGenericMethod,
					IsNullable = targetParameterType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated }
						or { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T },
					Parameters = parameters.ToEquatableReadOnlyList()!,
					Message = GetMessage(attribute),
				}
			);
		}

		var collectionPropertyDetails = propertyType switch
		{
			IArrayTypeSymbol ats =>
				GetPropertyValidations(
					compilation,
					propertyName,
					ats.ElementType,
					ats.ElementNullableAnnotation,
					attributes,
					token
				),

			INamedTypeSymbol
			{
				IsGenericType: true,
				TypeArguments: [{ } type],
				TypeArgumentNullableAnnotations: [{ } annotation],
			} nts when nts.AllInterfaces.Any(i => i.IsICollection1() || i.IsIReadOnlyCollection1()) =>
				GetPropertyValidations(
					compilation,
					propertyName,
					type,
					annotation,
					attributes,
					token
				),

			_ => null,
		};

		if (
			(isNullable || !isReferenceType)
			&& !isValidationProperty
			&& collectionPropertyDetails is null
			&& validations is []
		)
		{
			return null;
		}

		return new()
		{
			PropertyName = propertyName,
			TypeFullName = propertyType.ToDisplayString(s_fullyQualifiedPlusNullable),
			IsReferenceType = isReferenceType,
			IsNullable = isNullable,

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

	private static string? GetMessage(AttributeData attribute)
	{
		foreach (var p in attribute.NamedArguments)
		{
			if (p is { Key: "Message", Value.Value: string s })
				return $"\"{s}\"";
		}

		return null;
	}

	private static List<string> BuildParameterValues(AttributeData attribute, ImmutableArray<IParameterSymbol> parameters)
	{
		var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference!.GetSyntax();
		var argumentListSyntax = attributeSyntax.ArgumentList?.Arguments ?? [];

		var attributeParameters = attribute.AttributeConstructor!.Parameters;
		var attributeArguments = attribute.ConstructorArguments;
		var attributeNamedArguments = attribute.NamedArguments;
		List<IPropertySymbol>? attributeProperties = null;

		var parameterValues = new List<string>();

		for (var i = 0; i < argumentListSyntax.Count; i++)
		{
			switch (argumentListSyntax[i])
			{
				case { NameColon.Name.Identifier.ValueText: var name }:
				{
					for (var j = 0; j < attributeArguments.Length; j++)
					{
						if (attributeParameters[j].Name == name)
						{
							var parameterValue = BuildParameterValue(
								argumentListSyntax[i],
								attributeArguments[j],
								attributeParameters[j],
								parameters
							);
							parameterValues.Add(parameterValue);

							break;
						}
					}

					break;
				}

				case { NameEquals.Name.Identifier.ValueText: var name }:
				{
					if (name is "Message")
						break;

					var argument = attributeNamedArguments.First(a => a.Key == name).Value;

					attributeProperties ??= attribute.AttributeClass!.GetMembers()
						.OfType<IPropertySymbol>()
						.ToList();
					var property = attributeProperties.First(a => a.Name == name);

					var parameterValue = BuildParameterValue(
						argumentListSyntax[i],
						argument,
						property,
						parameters
					);
					parameterValues.Add(parameterValue);

					break;
				}

				default:
				{
					if (i < attributeParameters.Length
						&& i < attributeArguments.Length)
					{
						var parameterValue = BuildParameterValue(
							argumentListSyntax[i],
							attributeArguments[i],
							attributeParameters[i],
							parameters
						);
						parameterValues.Add(parameterValue);

					}

					break;
				}

			}
		}

		return parameterValues;
	}

	private static string BuildParameterValue(
		AttributeArgumentSyntax attributeArgumentSyntax,
		TypedConstant typedConstant,
		ISymbol parameterSymbol,
		ImmutableArray<IParameterSymbol> parameters
	)
	{
		var parameterName = GetParameterName(parameterSymbol.Name, parameters);
		var parameterValue = GetParameterValue(parameterSymbol, typedConstant, attributeArgumentSyntax);

		return $"{parameterName}: {parameterValue}";
	}

	private static string GetParameterName(string name, ImmutableArray<IParameterSymbol> parameters)
	{
		foreach (var p in parameters)
		{
			if (p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				return p.Name;
		}

		return name;
	}

	private static string GetParameterValue(ISymbol parameterSymbol, TypedConstant typedConstant, AttributeArgumentSyntax attributeArgumentSyntax)
	{
		if (parameterSymbol.IsTargetTypeSymbol()
			&& attributeArgumentSyntax.Expression.IsNameOfExpression(out var name))
		{
			return $"instance.{name}";
		}

		if (typedConstant is { Value: string s })
			return $"\"{s}\"";

		return typedConstant.Value?.ToString() ?? "";
	}
}

file static class Extensions
{
	public static bool IsTargetTypeSymbol(this ISymbol symbol) =>
		symbol is IParameterSymbol { Type.SpecialType: SpecialType.System_Object }
			or IPropertySymbol { Type.SpecialType: SpecialType.System_Object }
		&& symbol.GetAttributes().Any(a => a.AttributeClass.IsTargetTypeAttribute());

	public static bool IsNameOfExpression(this ExpressionSyntax syntax, out string? name)
	{
		name = null;
		if (syntax is InvocationExpressionSyntax
			{
				Expression: SimpleNameSyntax { Identifier.ValueText: "nameof" },
				ArgumentList.Arguments: [{ Expression: SimpleNameSyntax { Identifier.ValueText: var n } }],
			}
		)
		{
			name = n;
			return true;
		}
		else
		{
			return false;
		}
	}

	public static bool HasAdditionalValidationsMethod(this INamedTypeSymbol typeSymbol) =>
		typeSymbol.GetMembers()
			.OfType<IMethodSymbol>()
			.Any(m =>
				m is
				{
					Name: "AdditionalValidations",
					IsStatic: true,
					ReturnType: INamedTypeSymbol
					{
						ConstructedFrom: INamedTypeSymbol
						{
							SpecialType: SpecialType.System_Collections_Generic_IEnumerable_T,
						},
						TypeArguments:
						[
							INamedTypeSymbol
						{
							Name: "ValidationError",
							ContainingNamespace:
							{
								Name: "Shared",
								ContainingNamespace:
								{
									Name: "Validations",
									ContainingNamespace:
									{
										Name: "Immediate",
										ContainingNamespace.IsGlobalNamespace: true,
									},
								},
							},
						}
						],
					},
					Parameters: [{ Type: INamedTypeSymbol parameterType }],
				}
				&& SymbolEqualityComparer.Default.Equals(parameterType, typeSymbol)
			);
}
