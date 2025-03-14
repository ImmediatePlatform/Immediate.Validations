using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Validations.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValidateClassAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor ValidateAttributeMissing =
		new(
			id: DiagnosticIds.IV0012ValidateAttributeMissing,
			title: "Validation targets must be marked `[Validate]`",
			messageFormat: "Validation target `{0}` must be marked with `[Validate]`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Validation code can only be generated for classes using the `Validate` attribute."
		);

	public static readonly DiagnosticDescriptor IValidationTargetMissing =
		new(
			id: DiagnosticIds.IV0013IValidationTargetMissing,
			title: "Validation targets should implement the interface `IValidationTarget<>`",
			messageFormat: "Validation target `{0}` should declare that it implements `IValidationTarget<{0}>`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "ValidationBehavior<,> requires that classes declare implementation of `IValidationTarget<>` in order to be validated."
		);

	public static readonly DiagnosticDescriptor ValidatePropertyIncompatibleType =
		new(
			id: DiagnosticIds.IV0014ValidatePropertyIncompatibleType,
			title: "Validator will not be used",
			messageFormat: "Validator `{0}` will not be used for property `{1}` due to incompatible types",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Validators with incompatible types will be ignored.",
			customTags: [WellKnownDiagnosticTags.Unnecessary]
		);

	public static readonly DiagnosticDescriptor ValidateParameterIncompatibleType =
		new(
			id: DiagnosticIds.IV0015ValidateParameterIncompatibleType,
			title: "Parameter is incompatible type",
			messageFormat: "Property/parameter `{0}` is marked `[TargetType]`, but value is not of type `{1}`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Incompatible types will lead to incorrect validation code."
		);

	public static readonly DiagnosticDescriptor ValidateParameterNameofInvalid =
		new(
			id: DiagnosticIds.IV0017ValidateParameterNameofInvalid,
			title: "nameof() target is invalid",
			messageFormat: "nameof({0}) must refer to {1}",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Invalid nameof will generate incorrect code."
		);

	public static readonly DiagnosticDescriptor ValidateNotNullWhenInvalid =
		new(
			id: DiagnosticIds.IV0018ValidateNotNullWhenInvalid,
			title: "`[NotNull]` applied to not-null property",
			messageFormat: "`[NotNull]` is applied to property `{0}`, but type `{1}` cannot be null",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Invalid `[NotNull]` will generate incorrect code."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			ValidateAttributeMissing,
			IValidationTargetMissing,
			ValidatePropertyIncompatibleType,
			ValidateParameterIncompatibleType,
			ValidateParameterNameofInvalid,
			ValidateNotNullWhenInvalid,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSymbol,
			SyntaxKind.ClassDeclaration,
			SyntaxKind.RecordDeclaration,
			SyntaxKind.StructDeclaration,
			SyntaxKind.RecordStructDeclaration,
			SyntaxKind.InterfaceDeclaration
		);
	}

	private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var symbol = (INamedTypeSymbol)context.ContainingSymbol!;

		var hasValidateAttribute = symbol
			.GetAttributes()
			.Any(a => a.AttributeClass.IsValidateAttribute());

		var isIValidationTarget = symbol
			.Interfaces
			.Any(i => i.IsIValidationTarget());

		token.ThrowIfCancellationRequested();

		if (!hasValidateAttribute)
		{
			if (!isIValidationTarget && !symbol.HasValidatedProperties())
				return;

			context.ReportDiagnostic(
				Diagnostic.Create(
					ValidateAttributeMissing,
					symbol.Locations[0],
					symbol.Name
				)
			);
		}

		if (!isIValidationTarget)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					IValidationTargetMissing,
					symbol.Locations[0],
					symbol.Name
				)
			);
		}

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

		foreach (var property in symbol.GetMembers().OfType<IPropertySymbol>())
		{
			if (property is { IsStatic: true } or { DeclaredAccessibility: not Accessibility.Public })
				continue;

			foreach (var attribute in property.GetAttributes())
			{
				var status = ValidateAttribute(context.Compilation, property.Type, attribute.AttributeClass!, token);

				if (status.IncompatibleType)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							ValidatePropertyIncompatibleType,
							attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
							attribute.AttributeClass!.Name,
							property.Name
						)
					);
				}
				else if (status.InvalidNotNull)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							ValidateNotNullWhenInvalid,
							attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
							property.Name,
							property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
						)
					);
				}
				else if (status.ValidateArguments)
				{
					ValidateArguments(
						context,
						members,
						attribute,
						status.ValidateParameterSymbols,
						property.Type
					);
				}
			}
		}
	}

	private sealed record AttributeValidationStatus
	{
		public bool IncompatibleType { get; init; }
		public bool InvalidNotNull { get; init; }
		public bool ValidateArguments { get; init; }
		public ImmutableArray<IParameterSymbol> ValidateParameterSymbols { get; init; }
	}

	private static AttributeValidationStatus ValidateAttribute(
		Compilation compilation,
		ITypeSymbol propertyType,
		INamedTypeSymbol attributeSymbol,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		if (!attributeSymbol.ImplementsValidatorAttribute())
			return new();

		token.ThrowIfCancellationRequested();

		if (attributeSymbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.IsValidValidatePropertyMethod())
				.SingleValue() is not
				{
					Parameters: [{ Type: { } targetParameterType }, ..],
				} validateMethod
		)
		{
			// covered by other analyzers
			return new();
		}

		token.ThrowIfCancellationRequested();

		if (targetParameterType is ITypeParameterSymbol tps)
		{
			if (attributeSymbol.IsNotNullAttribute()
				&& propertyType is { IsReferenceType: false, OriginalDefinition.SpecialType: not SpecialType.System_Nullable_T })
			{
				return new() { InvalidNotNull = true };
			}

			if (tps.SatisfiesConstraints(propertyType, compilation))
			{
				return new()
				{
					ValidateArguments = true,
					ValidateParameterSymbols = validateMethod.Parameters,
				};
			}
		}
		else
		{
			var baseType = propertyType is { IsReferenceType: false, SpecialType: SpecialType.System_Nullable_T }
				? ((INamedTypeSymbol)propertyType).TypeArguments[0]
				: propertyType;

			var conversion = compilation.ClassifyConversion(baseType, targetParameterType);
			if (conversion is { IsIdentity: true }
					or { IsImplicit: true, IsReference: true }
					or { IsImplicit: true, IsNullable: true }
					or { IsBoxing: true }
			)
			{
				return new()
				{
					ValidateArguments = true,
					ValidateParameterSymbols = validateMethod.Parameters,
				};
			}
		}

		token.ThrowIfCancellationRequested();

		return propertyType switch
		{
			IArrayTypeSymbol ats =>
				ValidateAttribute(
					compilation,
					ats.ElementType,
					attributeSymbol,
					token
				),

			INamedTypeSymbol
			{
				IsGenericType: true,
				TypeArguments: [{ } type],
				TypeArgumentNullableAnnotations: [{ } annotation],
			} nts when nts.AllInterfaces.Any(i => i.IsICollection1() || i.IsIReadOnlyCollection1()) =>
				ValidateAttribute(
					compilation,
					type,
					attributeSymbol,
					token
				),

			_ => new() { IncompatibleType = true, },
		};
	}

	private static void ValidateArguments(
		SyntaxNodeAnalysisContext context,
		List<ISymbol> members,
		AttributeData attribute,
		ImmutableArray<IParameterSymbol> validateParameterSymbols,
		ITypeSymbol typeArgumentType
	)
	{
		var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference!.GetSyntax();
		var argumentListSyntax = attributeSyntax.ArgumentList?.Arguments ?? [];

		if (argumentListSyntax.Count == 0)
			return;

		var attributeParameters = attribute.AttributeConstructor!.Parameters;
		var attributeParameterIndex = 0;

		List<IPropertySymbol>? attributeProperties = null;

		for (var i = 0; i < argumentListSyntax.Count; i++)
		{
			switch (argumentListSyntax[i])
			{
				case { NameEquals.Name.Identifier.ValueText: var name }:
				{
					if (name is "Message")
						break;

					attributeProperties ??= [.. attribute.AttributeClass!.GetMembers().OfType<IPropertySymbol>()];

					var property = attributeProperties
						.First(a => string.Equals(a.Name, name, StringComparison.Ordinal));

					ValidateArgument(
						context,
						argumentListSyntax[i],
						property,
						validateParameterSymbols,
						typeArgumentType,
						members
					);

					break;
				}

				case { NameColon.Name.Identifier.ValueText: var name }:
				{
					for (var j = 0; j < attributeParameters.Length; j++)
					{
						if (string.Equals(attributeParameters[j].Name, name, StringComparison.Ordinal))
						{
							ValidateArgument(
								context,
								argumentListSyntax[i],
								attributeParameters[j],
								validateParameterSymbols,
								typeArgumentType,
								members
							);

							break;
						}
					}

					attributeParameterIndex++;
					break;
				}

				default:
				{
					var attributeParameter = attributeParameters[attributeParameterIndex];
					ValidateArgument(
						context,
						argumentListSyntax[i],
						attributeParameter,
						validateParameterSymbols,
						typeArgumentType,
						members
					);

					if (!attributeParameter.IsParams)
						attributeParameterIndex++;

					break;
				}
			}
		}
	}

	private static void ValidateArgument(
		SyntaxNodeAnalysisContext context,
		AttributeArgumentSyntax syntax,
		ISymbol parameter,
		ImmutableArray<IParameterSymbol> validateParameterSymbols,
		ITypeSymbol typeArgumentType,
		List<ISymbol> members
	)
	{
		if (!parameter.IsTargetTypeSymbol())
			return;

		var validateParameter = validateParameterSymbols
			.First(p => string.Equals(p.Name, parameter.Name, StringComparison.Ordinal));

		var argumentType = GetArgumentType(
			context,
			syntax,
			members
		);

		if (argumentType is null)
			return;

		if (ValidateArgumentType(context.Compilation, validateParameter, argumentType, typeArgumentType, out var targetType))
			return;

		context.ReportDiagnostic(
			Diagnostic.Create(
				ValidateParameterIncompatibleType,
				syntax.GetLocation(),
				parameter.Name,
				targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
			)
		);
	}

	private static ITypeSymbol? GetArgumentType(
		SyntaxNodeAnalysisContext context,
		AttributeArgumentSyntax syntax,
		List<ISymbol> members
	)
	{
		if (syntax.Expression.IsNameOfExpression(out var argumentExpression))
		{
			var argumentSymbol = GetArgumentSymbol(
				context,
				syntax,
				argumentExpression,
				members
			);

			if (argumentSymbol is null)
				return null;

			var argumentType = argumentSymbol switch
			{
				IMethodSymbol { ReturnType: { } t } => t,
				IPropertySymbol { Type: { } t } => t,
				IFieldSymbol { Type: { } t } => t,
				_ => throw new InvalidOperationException(),
			};

			return argumentType;
		}
		else
		{
			if (context.SemanticModel.GetOperation(syntax.Expression)?.Type is not ITypeSymbol argumentType)
				return null;

			return argumentType;
		}
	}

	private static ISymbol? GetArgumentSymbol(
		SyntaxNodeAnalysisContext context,
		AttributeArgumentSyntax syntax,
		ExpressionSyntax argumentExpression,
		List<ISymbol> members
	)
	{
		if (argumentExpression is SimpleNameSyntax { Identifier.ValueText: { } name })
		{
			var member = members
				.Find(p => string.Equals(p.Name, name, StringComparison.Ordinal));

			if (member is null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateParameterNameofInvalid,
						syntax.GetLocation(),
						name,
						FormattableString.Invariant($"a property or method on the class `{context.ContainingSymbol!.Name}`")
					)
				);

				return null;
			}

			return member;
		}
		else
		{
			var symbolInfo = context.SemanticModel.GetSymbolInfo(argumentExpression);

			var symbol = symbolInfo.Symbol
				?? symbolInfo.CandidateSymbols
					.FirstOrDefault(
						ims => ims is IMethodSymbol
						{
							Parameters: []
						}
					);

			if (symbol is null)
				return null;

			if (symbol is not { Kind: SymbolKind.Field or SymbolKind.Method or SymbolKind.Property })
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateParameterNameofInvalid,
						syntax.GetLocation(),
						symbol.Name,
						FormattableString.Invariant($"a property or method on the class `{context.ContainingSymbol!.Name}` or a static member of another class")
					)
				);

				return null;
			}

			if (symbol is not { IsStatic: true })
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateParameterNameofInvalid,
						syntax.GetLocation(),
						symbol.Name,
						FormattableString.Invariant($"a static member of the class `{symbol.ContainingType.Name}`")
					)
				);

				return null;
			}

			return symbol;
		}
	}

	private static bool ValidateArgumentType(
		Compilation compilation,
		IParameterSymbol parameter,
		ITypeSymbol argumentType,
		ITypeSymbol typeArgumentType,
		out ITypeSymbol targetType
	)
	{
		targetType = parameter.Type;
		var buildArrayTypeSymbol = false;

		if (parameter.IsParams
			&& targetType is IArrayTypeSymbol { ElementType: { } paramElementType })
		{
			targetType = paramElementType;

			if (argumentType is IArrayTypeSymbol { ElementType: { } argumentElementType })
			{
				argumentType = argumentElementType;
				buildArrayTypeSymbol = true;
			}
		}

		if (targetType is ITypeParameterSymbol)
			targetType = typeArgumentType;

		if (compilation.ClassifyConversion(argumentType, targetType).IsValidConversion())
			return true;

		if (buildArrayTypeSymbol)
			targetType = compilation.CreateArrayTypeSymbol(targetType);

		return false;
	}
}

file static class Extensions
{
	public static bool IsTargetTypeSymbol(this ISymbol symbol) =>
		symbol.GetAttributes().Any(a => a.AttributeClass.IsTargetTypeAttribute());

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

	public static bool HasValidatedProperties(this INamedTypeSymbol symbol) =>
		symbol
			.GetAllMembers()
			.Any(
				s => s is IPropertySymbol
					&& s.GetAttributes()
						.Any(a => a.AttributeClass.ImplementsValidatorAttribute())
			);
}
