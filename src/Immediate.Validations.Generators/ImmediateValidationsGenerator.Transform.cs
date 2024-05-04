using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Validations.Generators;

public sealed partial class ImmediateValidationsGenerator
{
	private static ValidationClass? TransformMethod(
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

	private static EquatableReadOnlyList<ValidationProperty> GetProperties(
		Compilation compilation,
		INamedTypeSymbol symbol,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var properties = new List<ValidationProperty>();
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

			properties.AddRange(
				GetPropertyValidations(
					compilation,
					property.Name,
					property.Type,
					property.NullableAnnotation,
					property.GetAttributes(),
					token
				)
			);
		}

		return properties.ToEquatableReadOnlyList();
	}

	private static IEnumerable<ValidationProperty> GetPropertyValidations(
		Compilation compilation,
		string propertyName,
		ITypeSymbol propertyType,
		NullableAnnotation nullableAnnotation,
		ImmutableArray<AttributeData> attributes,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		string? propertyTypeFullName = null;

		string GetPropertyTypeFullName() =>
			propertyTypeFullName ??= propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		if (propertyType.IsReferenceType is true
			&& nullableAnnotation is NullableAnnotation.NotAnnotated)
		{
			yield return new()
			{
				PropertyName = propertyName,
				TypeFullName = GetPropertyTypeFullName(),
				ValidatorName = $"global::Immediate.Validations.Shared.NotNullAttribute",
				IsGenericMethod = true,
				IsValidationProperty = false,
				Parameters = [],
				Message = null,
				CollectionProperties = [],
			};
		}

		token.ThrowIfCancellationRequested();

		if (propertyType.TypeKind is TypeKind.Enum)
		{
			yield return new()
			{
				PropertyName = propertyName,
				TypeFullName = GetPropertyTypeFullName(),
				ValidatorName = "global::Immediate.Validations.Shared.EnumValueAttribute",
				IsGenericMethod = true,
				IsValidationProperty = false,
				Parameters = [],
				Message = null,
				CollectionProperties = [],
			};
		}

		token.ThrowIfCancellationRequested();

		if (propertyType.GetAttributes().Any(v => v.AttributeClass.IsValidateAttribute()))
		{
			yield return new()
			{
				PropertyName = propertyName,
				TypeFullName = GetPropertyTypeFullName(),
				ValidatorName = "",
				IsGenericMethod = false,
				IsValidationProperty = true,
				Parameters = [],
				Message = null,
				CollectionProperties = [],
			};
		}

		token.ThrowIfCancellationRequested();

		foreach (var attribute in attributes)
		{
			token.ThrowIfCancellationRequested();

			var @class = attribute.AttributeClass?.OriginalDefinition;
			if (!@class.ImplementsValidatorAttribute())
				continue;

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

			if (targetParameterType is ITypeParameterSymbol tps)
			{
				if (!Utility.SatisfiesConstraints(validateMethod, [propertyType], compilation))
					continue;
			}
			else
			{
				var conversion = compilation.ClassifyConversion(propertyType, targetParameterType);
				if (conversion is not { IsIdentity: true } or { IsImplicit: true, IsReference: true } or { IsBoxing: true })
					continue;
			}

			var parameters = validateMethod.Parameters
				.Skip(1)
				.Select(p => GetValue(p, attribute))
				.Where(p => p is not null)
				.ToList();

			if (parameters.Count != validateMethod.Parameters.Length - 1)
				continue;

			yield return new()
			{
				PropertyName = propertyName,
				TypeFullName = validateMethod.IsGenericMethod ? GetPropertyTypeFullName() : "",
				ValidatorName = @class.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
				IsGenericMethod = validateMethod.IsGenericMethod,
				IsValidationProperty = false,
				Parameters = parameters.ToEquatableReadOnlyList()!,
				Message = GetMessage(attribute),
				CollectionProperties = [],
			};
		}

		switch (propertyType)
		{
			case IArrayTypeSymbol ats:
				token.ThrowIfCancellationRequested();

				yield return new()
				{
					PropertyName = propertyName,
					TypeFullName = "",
					ValidatorName = "",
					IsGenericMethod = false,
					IsValidationProperty = false,
					Message = null,
					Parameters = [],
					CollectionProperties = GetPropertyValidations(
						compilation,
						propertyName,
						ats.ElementType,
						ats.ElementNullableAnnotation,
						attributes,
						token
					).ToEquatableReadOnlyList(),
				};

				break;

			case INamedTypeSymbol
			{
				IsGenericType: true,
				TypeArguments: [{ } type],
				TypeArgumentNullableAnnotations: [{ } annotation],
			} nts when nts.AllInterfaces.Any(i => i.IsICollection1() || i.IsIReadOnlyCollection1()):
				token.ThrowIfCancellationRequested();

				yield return new()
				{
					PropertyName = propertyName,
					TypeFullName = "",
					ValidatorName = "",
					IsGenericMethod = false,
					IsValidationProperty = false,
					Message = null,
					Parameters = [],
					CollectionProperties = GetPropertyValidations(
						compilation,
						propertyName,
						type,
						annotation,
						attributes,
						token
					).ToEquatableReadOnlyList(),
				};

				break;

			default:
				break;
		}
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
