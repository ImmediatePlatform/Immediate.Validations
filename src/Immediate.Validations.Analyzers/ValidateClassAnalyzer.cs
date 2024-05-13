using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Validations.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValidateClassAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor ValidateAttributeMissing =
		new(
			id: DiagnosticIds.IV0011ValidateAttributeMissing,
			title: "Validation targets must be marked `[Validate]`",
			messageFormat: "Validation target `{0}` must be marked with `[Validate]`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Validation code can only be generated for classes using the `Validate` attribute."
		);

	public static readonly DiagnosticDescriptor IValidationTargetMissing =
		new(
			id: DiagnosticIds.IV0012IValidationTargetMissing,
			title: "Validation targets should implement the interface `IValidationTarget<>`",
			messageFormat: "Validation target `{0}` should declare that it implements `IValidationTarget<{0}>`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "ValidationBehavior<,> requires that classes declare implementation of `IValidationTarget<>` in order to be validated."
		);

	public static readonly DiagnosticDescriptor ValidatePropertyIncompatibleType =
		new(
			id: DiagnosticIds.IV0013ValidatePropertyIncompatibleType,
			title: "Validator will not be used",
			messageFormat: "Validator `{0}` will not be used for property `{1}` due to incompatible types",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Validators with incompatible types will be ignored.",
			customTags: [WellKnownDiagnosticTags.Unnecessary]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create<DiagnosticDescriptor>(
		[
			ValidateAttributeMissing,
			IValidationTargetMissing,
			ValidatePropertyIncompatibleType,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var symbol = (INamedTypeSymbol)context.Symbol;

		var hasValidateAttribute = symbol
			.GetAttributes()
			.Any(a => a.AttributeClass.IsValidateAttribute());

		var isIValidationTarget = symbol
			.Interfaces
			.Any(i => i.IsIValidationTarget());

		if (!hasValidateAttribute && !isIValidationTarget)
			return;

		token.ThrowIfCancellationRequested();

		if (!hasValidateAttribute)
		{
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

		var properties = symbol
			.GetMembers()
			.OfType<IPropertySymbol>();

		foreach (var property in properties)
		{
			foreach (var attribute in property.GetAttributes())
			{
				if (!IsAttributeValid(context.Compilation, property.Type, attribute.AttributeClass!, token))
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
			}
		}
	}

	private static bool IsAttributeValid(
		Compilation compilation,
		ITypeSymbol propertyType,
		INamedTypeSymbol attributeSymbol,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		if (!attributeSymbol.ImplementsValidatorAttribute())
			return true;

		token.ThrowIfCancellationRequested();

		if (attributeSymbol
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
			// covered by other analyzers
			return true;
		}

		if (targetParameterType is ITypeParameterSymbol tps)
		{
			if (Utility.SatisfiesConstraints(tps, propertyType, compilation))
				return true;
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
				return true;
			}
		}

		return propertyType switch
		{
			IArrayTypeSymbol ats =>
				IsAttributeValid(
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
				IsAttributeValid(
					compilation,
					type,
					attributeSymbol,
					token
				),

			_ => false,
		};
	}
}
