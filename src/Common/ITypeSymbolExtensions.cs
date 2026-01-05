using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Immediate.Validations;

internal static class ITypeSymbolExtensions
{
	public static bool IsICollection1(this INamedTypeSymbol typeSymbol) =>
		typeSymbol is
		{
			Arity: 1,
			Name: "ICollection",
			ContainingNamespace.IsSystemCollectionsGeneric: true,
		};

	public static bool IsIReadOnlyCollection1(this INamedTypeSymbol typeSymbol) =>
		typeSymbol is
		{
			Arity: 1,
			Name: "IReadOnlyCollection",
			ContainingNamespace.IsSystemCollectionsGeneric: true,
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

	public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type) =>
		type is { TypeKind: TypeKind.Interface }
			? type
				.AllInterfaces
				.SelectMany(i => i.GetMembers())
				.Concat(type.GetMembers())
			: type
				.GetBaseTypesAndThis()
				.SelectMany(t => t.GetMembers());

	public static bool IsValidateAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "ValidateAttribute",
			ContainingNamespace.IsImmediateValidationsShared: true,
		};

	public static bool IsValidatorAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "ValidatorAttribute",
			ContainingNamespace.IsImmediateValidationsShared: true,
		};

	public static bool ImplementsValidatorAttribute([NotNullWhen(true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol.IsValidatorAttribute()
		|| (typeSymbol?.BaseType is not null && ImplementsValidatorAttribute(typeSymbol.BaseType.OriginalDefinition));

	public static bool IsTargetTypeAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Arity: 0,
			Name: "TargetTypeAttribute",
			ContainingNamespace.IsImmediateValidationsShared: true,
		};

	public static bool IsNotNullAttribute([NotNullWhen(returnValue: true)] this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "NotNullAttribute",
			ContainingNamespace.IsImmediateValidationsShared: true,
		};

	public static bool IsValidationResult(this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Arity: 0,
			Name: "ValidationResult",
			ContainingNamespace.IsImmediateValidationsShared: true,
		};

	public static bool IsIValidationTarget(this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Arity: 1,
			Name: "IValidationTarget",
			ContainingNamespace.IsImmediateValidationsShared: true,
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
			SpecialType: SpecialType.System_Boolean,
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

	public static bool HasAdditionalValidationsMethod(this INamedTypeSymbol typeSymbol) =>
		typeSymbol.GetMembers()
			.OfType<IMethodSymbol>()
			.Any(m =>
				m is
				{
					Arity: 0,
					Name: "AdditionalValidations",
					IsStatic: true,
					ReturnsVoid: true,
					Parameters:
					[
					{ Type: INamedTypeSymbol validationResult },
					{ Type: INamedTypeSymbol parameterType },
					],
				}
				&& validationResult.IsValidationResult()
				&& SymbolEqualityComparer.Default.Equals(parameterType, typeSymbol)
			);

	public static bool IsDescriptionAttribute([NotNullWhen(returnValue: true)] this INamedTypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Arity: 0,
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
			Arity: 2,
			Name: "ValidationBehavior",
			ContainingNamespace.IsImmediateValidationsShared: true,
		};

	public static bool IsBehaviorsAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "BehaviorsAttribute",
			ContainingNamespace.IsImmediateHandlersShared: true,
		};

	public static bool IsAllowNullAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Name: "AllowNullAttribute",
			ContainingNamespace:
			{
				Name: "CodeAnalysis",
				ContainingNamespace:
				{
					Name: "Diagnostics",
					ContainingNamespace:
					{
						Name: "System",
						ContainingNamespace.IsGlobalNamespace: true,
					}
				}
			}
		};

	public static IReadOnlyList<(string? Target, IObjectCreationOperation Attribute)> GetTargetedAttributes(
		this IPropertySymbol propertySymbol,
		SemanticModel semanticModel
	)
	{
		switch (propertySymbol.DeclaringSyntaxReferences.First().GetSyntax())
		{
			case PropertyDeclarationSyntax propertySyntax:
			{
				var list = new List<(string? Target, IObjectCreationOperation AttributeOperation)>();

				foreach (var attributeList in propertySyntax.AttributeLists)
				{
					var target = attributeList.Target?.Identifier.ValueText;

					foreach (var attribute in attributeList.Attributes)
					{
						if (semanticModel.GetOperation(attribute) is IAttributeOperation { Operation: IObjectCreationOperation operation })
							list.Add((target, operation));
					}
				}

				return list;
			}

			case ParameterSyntax parameterSyntax:
			{
				var list = new List<(string? Target, IObjectCreationOperation AttributeOperation)>();

				foreach (var attributeList in parameterSyntax.AttributeLists)
				{
					var target = attributeList.Target?.Identifier.ValueText.NullIf("property");

					foreach (var attribute in attributeList.Attributes)
					{
						if (semanticModel.GetOperation(attribute) is IAttributeOperation { Operation: IObjectCreationOperation operation })
							list.Add((target, operation));
					}
				}

				return list;
			}

			case var syntax:
				throw new InvalidOperationException($"Property declared using a `{syntax.GetType().FullName}`.");
		}
	}

	public static bool IsTargetTypeSymbol(this ISymbol symbol) =>
		symbol is IParameterSymbol { Type.IsValidTargetTypeType: true }
			or IPropertySymbol { Type.IsValidTargetTypeType: true }
		&& symbol.GetAttributes().Any(a => a.AttributeClass.IsTargetTypeAttribute());

	extension(ITypeSymbol typeSymbol)
	{
		[SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Roslyn Bug (https://github.com/dotnet/roslyn/issues/81213)")]
		private bool IsValidTargetTypeType =>
			typeSymbol is { SpecialType: SpecialType.System_Object or SpecialType.System_String }
				or IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Object or SpecialType.System_String };
	}

	extension(INamespaceSymbol namespaceSymbol)
	{
		public bool IsImmediateValidationsShared =>
			namespaceSymbol is
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
			};

		public bool IsImmediateHandlersShared =>
			namespaceSymbol is
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
			};

		public bool IsSystemCollectionsGeneric =>
			namespaceSymbol is
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
			};
	}
}
