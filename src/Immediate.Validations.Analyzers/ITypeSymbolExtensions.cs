using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Immediate.Validations.Analyzers;

internal static class ITypeSymbolExtensions
{
	public static bool IsValidatorAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
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

	public static bool IsIValidationTarget(this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			MetadataName: "IValidationTarget`1",
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

	public static bool IsIReadOnlyCollection1(this INamedTypeSymbol typeSymbol) =>
		typeSymbol is
		{
			MetadataName: "IReadOnlyCollection`1",
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

	public static bool IsBehaviorsAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Name: "BehaviorsAttribute",
			ContainingNamespace:
			{
				Name: "Shared",
				ContainingNamespace:
				{
					Name: "Handlers",
					ContainingNamespace:
					{
						Name: "Immediate",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			},
		};

	public static bool IsValidationBehavior([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			MetadataName: "ValidationBehavior`2",
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

	public static bool IsTargetTypeAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Name: "TargetTypeAttribute",
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
}
