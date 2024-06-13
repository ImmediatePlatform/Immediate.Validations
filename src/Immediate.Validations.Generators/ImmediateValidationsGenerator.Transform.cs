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
		var baseValidationTargets = GetBaseValidationTargets(symbol);
		var properties = GetProperties(context.SemanticModel, symbol, token);

		return new()
		{
			Namespace = @namespace,
			OuterClasses = outerClasses,
			Class = GetClass(symbol),
			HasAdditionalValidationsMethod = symbol.HasAdditionalValidationsMethod(),
			IsReferenceType = symbol.IsReferenceType,
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

	private static EquatableReadOnlyList<string> GetBaseValidationTargets(INamedTypeSymbol symbol)
	{
		List<string>? baseValidationTargets = null;

		if (symbol.BaseType.IsValidationTarget())
			(baseValidationTargets = []).Add(symbol.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

		foreach (var i in symbol.Interfaces)
		{
			if (i.IsValidationTarget())
				(baseValidationTargets ??= []).Add(i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
		}

		if (baseValidationTargets is null)
			return default;

		return baseValidationTargets.ToEquatableReadOnlyList();
	}

	private static EquatableReadOnlyList<ValidationTargetProperty> GetProperties(
		SemanticModel semanticModel,
		INamedTypeSymbol symbol,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var members = symbol
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

		var properties = new List<ValidationTargetProperty>();
		foreach (var member in symbol.GetMembers())
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

			if (symbol.TypeKind is not TypeKind.Interface && property.SetMethod is null)
				continue;

			token.ThrowIfCancellationRequested();

			if (GetPropertyValidations(
					semanticModel,
					members,
					property.GetDescription(),
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
		SemanticModel semanticModel,
		List<ISymbol> members,
		string propertyName,
		ITypeSymbol propertyType,
		NullableAnnotation nullableAnnotation,
		ImmutableArray<AttributeData> attributes,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var name = propertyName;
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

			var @class = attribute.AttributeClass;
			if (@class.IsDescriptionAttribute())
				continue;

			if (!@class.ImplementsValidatorAttribute())
				continue;

			token.ThrowIfCancellationRequested();

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
				continue;
			}

			token.ThrowIfCancellationRequested();

			if (targetParameterType is ITypeParameterSymbol tps)
			{
				if (!tps.SatisfiesConstraints(propertyType, semanticModel.Compilation))
					continue;
			}
			else
			{
				var conversion = semanticModel.Compilation
					.ClassifyConversion(baseType, targetParameterType);

				if (!conversion.IsValidConversion())
					continue;
			}

			token.ThrowIfCancellationRequested();

			var parameters = BuildParameterValues(
				semanticModel,
				members,
				attribute,
				validateMethod.Parameters
			);

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
					semanticModel,
					members,
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
					semanticModel,
					members,
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
			Name = name,
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

	private static string[]? BuildParameterValues(
		SemanticModel semanticModel,
		List<ISymbol> members,
		AttributeData attribute,
		ImmutableArray<IParameterSymbol> parameters
	)
	{
		var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference!.GetSyntax();
		var argumentListSyntax = attributeSyntax.ArgumentList?.Arguments ?? [];

		if (argumentListSyntax.Count == 0)
			return null;

		var attributeParameters = attribute.AttributeConstructor!.Parameters;
		var attributeParameterIndex = 0;
		List<IPropertySymbol>? attributeProperties = null;

		var count = GetArgumentCount(argumentListSyntax);

		var parameterValues = new string[count];
		var propertyValuesIndex = 0;
		var propertyParameterCount = 0;

		for (var i = 0; i < argumentListSyntax.Count; i++)
		{
			switch (argumentListSyntax[i])
			{
				case { NameEquals.Name.Identifier.ValueText: var name }:
				{
					if (name is "Message")
						break;

					if (propertyParameterCount > 0)
					{
						var remaining = count - i;
						Array.Copy(
							parameterValues,
							i - propertyParameterCount,
							parameterValues,
							count - propertyParameterCount,
							propertyParameterCount
						);

						propertyValuesIndex -= propertyParameterCount;
						propertyParameterCount = 0;
					}

					attributeProperties ??= attribute.AttributeClass!.GetMembers()
						.OfType<IPropertySymbol>()
						.ToList();
					var property = attributeProperties.First(a => a.Name == name);

					var parameterValue = BuildParameterValue(
						semanticModel,
						members,
						argumentListSyntax[i],
						property,
						parameters
					);
					parameterValues[propertyValuesIndex++] = parameterValue;

					break;
				}

				case { NameColon.Name.Identifier.ValueText: var name, Expression: { } expr }:
				{
					for (var j = 0; j < attributeParameters.Length; j++)
					{
						if (attributeParameters[j].Name == name)
						{
							var parameterValue = BuildParameterValue(
								semanticModel,
								members,
								argumentListSyntax[i],
								attributeParameters[j],
								parameters
							);

							parameterValues[propertyValuesIndex++] = parameterValue;
							break;
						}
					}

					attributeParameterIndex++;
					break;
				}

				default:
				{
					var attributeParameter = attributeParameters[attributeParameterIndex];
					if (!attributeParameter.IsParams)
					{
						var parameterValue = BuildParameterValue(
							semanticModel,
							members,
							argumentListSyntax[i],
							attributeParameter,
							parameters
						);
						parameterValues[propertyValuesIndex++] = parameterValue;
						attributeParameterIndex++;
					}
					else
					{
						var parameterValue = GetParameterValue(
							semanticModel,
							members,
							attributeParameter,
							argumentListSyntax[i]
						);

						parameterValues[propertyValuesIndex++] = parameterValue;
						propertyParameterCount++;
					}

					break;
				}
			}
		}

		return parameterValues;
	}

	private static int GetArgumentCount(SeparatedSyntaxList<AttributeArgumentSyntax> argumentListSyntax)
	{
		var count = argumentListSyntax.Count;
		if (argumentListSyntax.Any(a => a is { NameEquals.Name.Identifier.ValueText: "Message", }))
			return count - 1;
		return count;
	}

	private static string BuildParameterValue(
		SemanticModel semanticModel,
		List<ISymbol> members,
		AttributeArgumentSyntax attributeArgumentSyntax,
		ISymbol parameterSymbol,
		ImmutableArray<IParameterSymbol> parameters
	)
	{
		var parameterName = GetParameterName(parameterSymbol.Name, parameters);
		var parameterValue = GetParameterValue(semanticModel, members, parameterSymbol, attributeArgumentSyntax);

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

	private static string GetParameterValue(
		SemanticModel semanticModel,
		List<ISymbol> members,
		ISymbol parameterSymbol,
		AttributeArgumentSyntax attributeArgumentSyntax
	)
	{
		if (parameterSymbol.IsTargetTypeSymbol()
			&& attributeArgumentSyntax.Expression.IsNameOfExpression(out var name))
		{
			var member = members.FirstOrDefault(m => m.Name.Equals(name, StringComparison.Ordinal));

			return member switch
			{
				IMethodSymbol { IsStatic: true } => $"{name}()",
				IMethodSymbol => $"instance.{name}()",
				{ IsStatic: true } => $"{name}",
				_ => $"instance.{name}",
			};
		}

		var operation = semanticModel
			.GetOperation(attributeArgumentSyntax.Expression);

		return operation?.ConstantValue switch
		{
			{ HasValue: true, Value: string s } => $"@\"{s}\"",
			{ HasValue: true, Value: { } o } => o.ToString(),
			_ => "",
		};
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

	public static string GetDescription(this ISymbol symbol)
	{
		if (symbol is IMethodSymbol)
			return $"{symbol.Name}()";

		if (symbol is not (IFieldSymbol or IPropertySymbol))
			return symbol.Name;

		foreach (var attribute in symbol.GetAttributes())
		{
			if (attribute.AttributeClass.IsDescriptionAttribute()
				&& attribute.ConstructorArguments is [{ Value: string v }]
				&& !string.IsNullOrWhiteSpace(v))
			{
				return v;
			}
		}

		return symbol.Name;
	}
}
