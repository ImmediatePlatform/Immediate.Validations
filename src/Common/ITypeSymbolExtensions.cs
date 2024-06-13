using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Immediate.Validations;

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

	public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol? type)
	{
		var current = type;
		while (current != null)
		{
			yield return current;
			current = current.BaseType;
		}
	}

	public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
	{
		if (type is { TypeKind: TypeKind.Interface })
		{
			return type.AllInterfaces
				.SelectMany(i => i.GetMembers())
				.Concat(type.GetMembers());
		}
		else
		{
			return type
				.GetBaseTypesAndThis()
				.SelectMany(t => t.GetMembers());
		}
	}

	public static bool IsValidateAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
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

	public static bool IsValidationTarget([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is not null
		&& typeSymbol.Interfaces
			.Any(i =>
				i.IsIValidationTarget()
				&& SymbolEqualityComparer.Default.Equals(typeSymbol, i.TypeArguments[0])
			);

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

	public static bool IsValidValidatePropertyMethod(this IMethodSymbol methodSymbol) =>
		methodSymbol is
		{
			IsStatic: true,
			Parameters.Length: >= 1,
			Name: "ValidateProperty",
			ReturnType: { } returnType,
		}
		&& returnType.IsValidValidatorReturn();

	public static bool IsDescriptionAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Name: "DescriptionAttribute",
			ContainingNamespace:
			{
				Name: "ComponentModel",
				ContainingNamespace:
				{
					Name: "System",
					ContainingNamespace.IsGlobalNamespace: true,
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
}
