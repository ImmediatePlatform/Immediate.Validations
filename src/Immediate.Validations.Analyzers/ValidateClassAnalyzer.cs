using System.Collections.Immutable;
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
			messageFormat: "Property/parameter `{0}` is marked `[TargetType]`, but value is not of the type `{1}`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Incompatible types will lead to incorrect validation code."
		);

	public static readonly DiagnosticDescriptor ValidateParameterPropertyIncompatibleType =
		new(
			id: DiagnosticIds.IV0016ValidateParameterPropertyIncompatibleType,
			title: "Parameter is incompatible type",
			messageFormat: "Property/parameter `{0}` is marked `[TargetType]`, but property `{1}` is not of the type `{2}`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Incompatible types will lead to incorrect validation code."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create<DiagnosticDescriptor>(
		[
			ValidateAttributeMissing,
			IValidationTargetMissing,
			ValidatePropertyIncompatibleType,
			ValidateParameterIncompatibleType,
			ValidateParameterPropertyIncompatibleType,
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
			.OfType<IPropertySymbol>()
			.ToList();

		foreach (var property in properties)
		{
			foreach (var attribute in property.GetAttributes())
			{
				var status = ValidateAttribute(context.Compilation, property.Type, attribute.AttributeClass!, token);

				if (status.Report)
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
				else if (status.TargetType is not null)
				{
					ValidateArguments(context, properties, attribute, status.TargetType);
				}
			}
		}
	}

	private sealed record AttributeValidationStatus
	{
		public required bool Report { get; init; }
		public ITypeSymbol? TargetType { get; init; }
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
			return new() { Report = false };

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
			return new() { Report = false };
		}

		if (targetParameterType is ITypeParameterSymbol tps)
		{
			if (Utility.SatisfiesConstraints(tps, propertyType, compilation))
				return new() { Report = false, TargetType = propertyType };
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
				return new() { Report = false };
			}
		}

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

			_ => new() { Report = true, },
		};
	}

	private static void ValidateArguments(
		SymbolAnalysisContext context,
		List<IPropertySymbol> properties,
		AttributeData attribute,
		ITypeSymbol targetType
	)
	{
		var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference!.GetSyntax();
		var argumentListSyntax = attributeSyntax.ArgumentList?.Arguments ?? [];

		var attributeParameters = attribute.AttributeConstructor!.Parameters;
		var attributeArguments = attribute.ConstructorArguments;
		var attributeNamedArguments = attribute.NamedArguments;
		List<IPropertySymbol>? attributeProperties = null;

		for (var i = 0; i < argumentListSyntax.Count; i++)
		{
			switch (argumentListSyntax[i])
			{
				case { NameColon.Name.Identifier.ValueText: var name }:
				{
					for (var j = 0; j < attributeArguments.Length; j++)
					{
						if (attributeParameters[j].Name == name)
						{
							ValidateArgument(
								context,
								argumentListSyntax[i],
								attributeParameters[j],
								attributeArguments[j],
								targetType,
								properties
							);

							break;
						}
					}

					break;
				}

				case { NameEquals.Name.Identifier.ValueText: var name }:
				{
					var argument = attributeNamedArguments.First(a => a.Key == name).Value;

					attributeProperties ??= attribute.AttributeClass!.GetMembers()
						.OfType<IPropertySymbol>()
						.ToList();
					var property = attributeProperties.First(a => a.Name == name);

					ValidateArgument(
						context,
						argumentListSyntax[i],
						property,
						argument,
						targetType,
						properties
					);

					break;
				}

				default:
				{
					if (i < attributeParameters.Length
						&& i < attributeArguments.Length)
					{
						ValidateArgument(
							context,
							argumentListSyntax[i],
							attributeParameters[i],
							attributeArguments[i],
							targetType,
							properties
						);
					}

					break;
				}
			}
		}
	}

	private static void ValidateArgument(
		SymbolAnalysisContext context,
		AttributeArgumentSyntax syntax,
		ISymbol parameter,
		TypedConstant argument,
		ITypeSymbol targetType,
		List<IPropertySymbol> properties
	)
	{
		if (parameter.IsTargetTypeSymbol()
			&& argument.Type is not null
		)
		{
			if (syntax.Expression.IsNameOfExpression(out var propertyName))
			{
				var property = properties
					.FirstOrDefault(p => p.Name == propertyName);

				if (property is null)
					return;

				if (!context.Compilation.ClassifyConversion(property.Type, targetType).IsValidConversion())
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							ValidateParameterPropertyIncompatibleType,
							syntax.GetLocation(),
							parameter.Name,
							property.Name,
							targetType.Name
						)
					);
				}
			}
			else
			{
				if (!context.Compilation.ClassifyConversion(argument.Type, targetType).IsValidConversion())
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							ValidateParameterIncompatibleType,
							syntax.GetLocation(),
							parameter.Name,
							targetType.Name
						)
					);
				}
			}
		}
	}
}

file static class Extensions
{
	public static bool IsTargetTypeSymbol(this ISymbol symbol) =>
		symbol is IParameterSymbol { Type.SpecialType: SpecialType.System_Object }
			or IPropertySymbol { Type.SpecialType: SpecialType.System_Object }
		&& symbol.GetAttributes().Any(a => a.AttributeClass.IsTargetTypeAttribute());

	public static bool IsNameOfExpression(this ExpressionSyntax syntax, out string? name)
	{
		name = null;
		if (syntax is InvocationExpressionSyntax
			{
				Expression: SimpleNameSyntax { Identifier.ValueText: "nameof" },
				ArgumentList.Arguments: [{ Expression: SimpleNameSyntax { Identifier.ValueText: var n } }],
			}
		)
		{
			name = n;
			return true;
		}
		else
			return false;
	}
}
