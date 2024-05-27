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
			messageFormat: "Property/parameter `{0}` is marked `[TargetType]`, but value is not of type `{1}`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Incompatible types will lead to incorrect validation code."
		);

	public static readonly DiagnosticDescriptor ValidateParameterPropertyIncompatibleType =
		new(
			id: DiagnosticIds.IV0016ValidateParameterPropertyIncompatibleType,
			title: "Parameter is incompatible type",
			messageFormat: "Property/parameter `{0}` is marked `[TargetType]`, but property `{1}` is not of type `{2}`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Incompatible types will lead to incorrect validation code."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
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

		var members = symbol
			.GetMembers()
			.Where(m => m is IPropertySymbol
				or IMethodSymbol { Parameters: [] })
			.ToList();

		foreach (var property in members.OfType<IPropertySymbol>())
		{
			if (property is { IsStatic: true }) continue;

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
		public required bool Report { get; init; }
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
			{
				return new()
				{
					Report = false,
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
					Report = false,
					ValidateArguments = true,
					ValidateParameterSymbols = validateMethod.Parameters,
				};
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

					attributeProperties ??= attribute.AttributeClass!.GetMembers()
						.OfType<IPropertySymbol>()
						.ToList();
					var property = attributeProperties.First(a => a.Name == name);

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
						if (attributeParameters[j].Name == name)
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

		var validateParameter = validateParameterSymbols.First(p => p.Name == parameter.Name);
		var targetType = validateParameter.Type;

		if (validateParameter.IsParams && targetType is IArrayTypeSymbol { ElementType: { } et })
			targetType = et;

		if (targetType is ITypeParameterSymbol)
			targetType = typeArgumentType;

		if (syntax.Expression.IsNameOfExpression(out var propertyName))
		{
			var member = members
				.FirstOrDefault(p => p.Name == propertyName);

			if (member is null)
				return;

			var memberType = member switch
			{
				IPropertySymbol { Type: { } t } => t,
				IMethodSymbol { ReturnType: { } t } => t,
				_ => throw new InvalidOperationException(),
			};

			if (!context.Compilation.ClassifyConversion(memberType, targetType).IsValidConversion())
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateParameterPropertyIncompatibleType,
						syntax.GetLocation(),
						parameter.Name,
						member.Name,
						targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
					)
				);
			}
		}
		else
		{
			if (context.SemanticModel.GetOperation(syntax.Expression)?.Type is not ITypeSymbol argumentType)
				return;

			if (!context.Compilation.ClassifyConversion(argumentType, targetType).IsValidConversion())
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateParameterIncompatibleType,
						syntax.GetLocation(),
						parameter.Name,
						targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
					)
				);
			}
		}
	}
}

file static class Extensions
{
	public static bool IsTargetTypeSymbol(this ISymbol symbol) =>
		symbol.GetAttributes().Any(a => a.AttributeClass.IsTargetTypeAttribute());

	public static bool IsNameOfExpression(this ExpressionSyntax syntax, out string? name)
	{
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
		{
			name = null;
			return false;
		}
	}
}
