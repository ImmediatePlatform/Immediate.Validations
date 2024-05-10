using Microsoft.CodeAnalysis;

namespace Immediate.Validations.Analyzers;

internal static class ITypeSymbolExtensions
{
	public static bool IsValidator(this INamedTypeSymbol? typeSymbol) =>
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

	public static bool IsValidValidatorReturn(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
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
			{ SpecialType: SpecialType.System_String, NullableAnnotation: NullableAnnotation.Annotated or NullableAnnotation.None },
			]
		};
}
