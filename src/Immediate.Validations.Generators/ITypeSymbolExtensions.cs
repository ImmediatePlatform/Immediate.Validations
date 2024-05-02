using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Immediate.Validations.Generators;

internal static class ITypeSymbolExtensions
{
	public static bool IsICollection1(this INamedTypeSymbol typeSymbol) =>
		typeSymbol is
		{
			MetadataName: "ICollection`1",
			ContainingNamespace:
			{
				Name: "Generic",
				ContainingNamespace:
				{
					Name: "Collections",
					ContainingNamespace:
					{
						Name: "System",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			},
		};

	public static bool IsValidateAttribute(this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Name: "ValidateAttribute",
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
		};

	public static bool IsValidatorAttribute(this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Name: "ValidatorAttribute",
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
		};

	public static bool ImplementsValidatorAttribute([NotNullWhen(true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol.IsValidatorAttribute()
		|| (typeSymbol?.BaseType is not null && ImplementsValidatorAttribute(typeSymbol.BaseType.OriginalDefinition));
}
