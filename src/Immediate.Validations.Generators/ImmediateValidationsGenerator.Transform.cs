using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
		var properties = GetProperties(context.SemanticModel.Compilation, symbol, token);

		return new()
		{
			Namespace = @namespace,
			OuterClasses = outerClasses,
			Class = GetClass(symbol),
			IsReferenceType = symbol.IsReferenceType,
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
		var outerClasses = new List<Class>();
		var outerSymbol = symbol.ContainingType;
		while (outerSymbol is not null)
		{
			outerClasses.Add(GetClass(outerSymbol));
			outerSymbol = outerSymbol.ContainingType;
		}

		outerClasses.Reverse();

		return outerClasses.ToEquatableReadOnlyList();
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

			var parameters = validateMethod.Parameters
				.Skip(1)
				.Select(p => GetValue(p, attribute))
				.Where(p => p is not null)
				.ToList();

			if (parameters.Count != validateMethod.Parameters.Length - 1)
				continue;

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
			isNullable
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

	private static TypedConstant? GetValue(AttributeData attribute, string name)
	{
		foreach (var p in attribute.NamedArguments)
		{
			if (p.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
				return p.Value;
		}

		return null;
	}

	private static string? GetMessage(AttributeData attribute)
	{
		if (GetValue(attribute, "Message")?.Value is not string s)
			return null;

		return $"\"{s}\"";
	}

	private static string? GetValue(IParameterSymbol parameter, AttributeData attribute)
	{
		if (GetValue(attribute, parameter.Name) is not { } constant)
			return null;

		var value = constant.Value?.ToString();
		return $"{parameter.Name}: {value}";
	}
}
