using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

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
			token.ThrowIfCancellationRequested();

			if (property is { IsStatic: true } or { DeclaredAccessibility: not Accessibility.Public })
				continue;

			foreach (var (target, attribute) in property.GetTargetedAttributes(context.SemanticModel))
			{
				var targetType = target switch
				{
					"element" => property.Type.GetElementType(),
					_ => property.Type,
				};

				var status = ValidateAttribute(
					context.Compilation,
					targetType,
					(INamedTypeSymbol)attribute.Type!,
					token
				);

				if (status.IncompatibleType)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							ValidatePropertyIncompatibleType,
							attribute.Syntax.GetLocation(),
							attribute.Type!.Name,
							property.Name
						)
					);
				}
				else if (status.InvalidNotNull)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							ValidateNotNullWhenInvalid,
							attribute.Syntax.GetLocation(),
							property.Name,
							targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
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
						property.Type,
						token
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

			if (compilation.ClassifyConversion(baseType, targetParameterType).IsValidConversion())
			{
				return new()
				{
					ValidateArguments = true,
					ValidateParameterSymbols = validateMethod.Parameters,
				};
			}
		}

		return new() { IncompatibleType = true, };
	}

	private static void ValidateArguments(
		SyntaxNodeAnalysisContext context,
		List<ISymbol> members,
		IObjectCreationOperation attribute,
		ImmutableArray<IParameterSymbol> validateParameterSymbols,
		ITypeSymbol typeArgumentType,
		CancellationToken token
	)
	{
		if (attribute.Constructor is null)
			return;

		foreach (var argument in attribute.Arguments)
		{
			token.ThrowIfCancellationRequested();

			var constructorParameter = argument.Parameter!;

			if (constructorParameter.Type is not IArrayTypeSymbol { ElementType: { } })
			{
				if (argument.Syntax is AttributeArgumentSyntax aas)
				{
					ValidateArgument(
						context,
						aas.Expression,
						constructorParameter,
						validateParameterSymbols,
						typeArgumentType,
						members
					);
				}

				continue;
			}

			if (argument.Value is not IArrayCreationOperation
				{
					Initializer.ElementValues: { } elements,
				})
			{
				return;
			}

			foreach (var element in elements)
			{
				ValidateArgument(
					context,
					(ExpressionSyntax)element.Syntax,
					constructorParameter,
					validateParameterSymbols,
					typeArgumentType,
					members
				);
			}
		}

		if (attribute.Initializer?.Initializers is { } initializers)
		{
			foreach (var initializer in initializers)
			{
				token.ThrowIfCancellationRequested();

				if (initializer is not ISimpleAssignmentOperation
					{
						Target: IPropertyReferenceOperation { Property: { } property },
					})
				{
					return;
				}

				if (property.Name is "Message")
					continue;

				if (initializer.Syntax is not AttributeArgumentSyntax aas)
					return;

				ValidateArgument(
					context,
					aas.Expression,
					property,
					validateParameterSymbols,
					typeArgumentType,
					members
				);
			}
		}
	}

	private static void ValidateArgument(
		SyntaxNodeAnalysisContext context,
		ExpressionSyntax syntax,
		ISymbol parameter,
		ImmutableArray<IParameterSymbol> validateParameterSymbols,
		ITypeSymbol typeArgumentType,
		List<ISymbol> members
	)
	{
		if (!parameter.IsTargetTypeSymbol())
			return;

		var validateParameter = validateParameterSymbols
			.First(p => string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));

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
		ExpressionSyntax syntax,
		List<ISymbol> members
	)
	{
		if (syntax.IsNameOfExpression(out var argumentExpression))
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
			return context.SemanticModel.GetOperation(syntax, context.CancellationToken)?.Type switch
			{
				ITypeSymbol argumentType => argumentType,
				_ => null,
			};
		}
	}

	private static ISymbol? GetArgumentSymbol(
		SyntaxNodeAnalysisContext context,
		ExpressionSyntax argumentExpressionSyntax,
		ExpressionSyntax nameofExpression,
		List<ISymbol> members
	)
	{
		if (nameofExpression is SimpleNameSyntax { Identifier.ValueText: { } name })
		{
			var member = members
				.Find(p => string.Equals(p.Name, name, StringComparison.Ordinal));

			if (member is null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateParameterNameofInvalid,
						argumentExpressionSyntax.GetLocation(),
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
			var symbolInfo = context.SemanticModel.GetSymbolInfo(nameofExpression, context.CancellationToken);

			var symbol = symbolInfo.Symbol
				?? symbolInfo.CandidateSymbols
					.FirstOrDefault(
						ims => ims is IMethodSymbol
						{
							Parameters: [],
						}
					);

			if (symbol is null)
				return null;

			if (symbol is not { Kind: SymbolKind.Field or SymbolKind.Method or SymbolKind.Property })
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateParameterNameofInvalid,
						argumentExpressionSyntax.GetLocation(),
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
						argumentExpressionSyntax.GetLocation(),
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
	public static bool HasValidatedProperties(this INamedTypeSymbol symbol) =>
		symbol
			.GetAllMembers()
			.Any(
				s => s is IPropertySymbol
					&& s.GetAttributes().Any(a => a.AttributeClass.ImplementsValidatorAttribute())
			);

	public static ITypeSymbol GetElementType(this ITypeSymbol typeSymbol) =>
		typeSymbol switch
		{
			IArrayTypeSymbol { ElementType: { } elementType } => elementType.GetElementType(),

			INamedTypeSymbol
			{
				IsGenericType: true,
				TypeArguments: [{ } type],
			} nts when
				nts.IsICollection1()
				|| nts.IsIReadOnlyCollection1()
				|| nts.AllInterfaces.Any(i => i.IsICollection1() || i.IsIReadOnlyCollection1()) =>
				type.GetElementType(),

			_ => typeSymbol,
		};
}
