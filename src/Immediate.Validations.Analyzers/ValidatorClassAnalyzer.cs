using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Validations.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValidatorClassAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor ValidateMethodMustExist =
		new(
			id: DiagnosticIds.IV0001ValidateMethodMustExist,
			title: "Validators must have a valid `ValidateProperty` method",
			messageFormat: "Validator class `{0}` must have a `ValidateProperty` method",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Generated code expects to be able to call a `ValidateProperty` method on the validator class."
		);

	public static readonly DiagnosticDescriptor ValidateMethodMustBeStatic =
		new(
			id: DiagnosticIds.IV0002ValidateMethodMustBeStatic,
			title: "`ValidateProperty` method must be static",
			messageFormat: "`ValidateProperty` method must be static",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Generated code expects to be able to call a `ValidateProperty` method on the validator class."
		);

	public static readonly DiagnosticDescriptor ValidateMethodMustBeUnique =
		new(
			id: DiagnosticIds.IV0003ValidateMethodMustBeUnique,
			title: "`ValidateProperty` method must be unique",
			messageFormat: "`ValidateProperty` method must be unique",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Generated code expects to be able to call a `ValidateProperty` method on the validator class."
		);

	public static readonly DiagnosticDescriptor ValidateMethodMustReturnValueTuple =
		new(
			id: DiagnosticIds.IV0004ValidateMethodMustReturnValueTuple,
			title: "`ValidateProperty` method must have a valid return",
			messageFormat: "`ValidateProperty` method must return `bool`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Generated code expects to be able to call a `ValidateProperty` method on the validator class."
		);

	public static readonly DiagnosticDescriptor ValidateMethodIsMissingParameter =
		new(
			id: DiagnosticIds.IV0005ValidateMethodIsMissingParameter,
			title: "`ValidateProperty` method is missing parameters",
			messageFormat: "Property `{0}` does not have a matching parameter `{1}` on the `ValidateProperty` method",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "`Validate()` must receive every property present on the containing Validator class."
		);

	public static readonly DiagnosticDescriptor ValidateMethodHasExtraParameter =
		new(
			id: DiagnosticIds.IV0006ValidateMethodHasExtraParameter,
			title: "`ValidateProperty` method has extra parameters",
			messageFormat: "Parameter `{0}` does not have a matching property `{1}`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "`Validate()` parameters must exist as properties on the containing Validator class."
		);

	public static readonly DiagnosticDescriptor ValidateMethodParameterIsIncorrectType =
		new(
			id: DiagnosticIds.IV0007ValidateMethodParameterIsIncorrectType,
			title: "`ValidateProperty` parameters and Validator properties must match",
			messageFormat: "Parameter `{0}` and matching property `{1}` have different types",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "`Validate()` parameters must exist as properties on the containing Validator class."
		);

	public static readonly DiagnosticDescriptor ValidatePropertyMustBeRequired =
		new(
			id: DiagnosticIds.IV0008ValidatePropertyMustBeRequired,
			title: "Validator property must be `required`",
			messageFormat: "Property/parameter `{0}` must have the `required` modifier",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "`Validate()` parameters without a default value require values to be set on their matching properties."
		);

	public static readonly DiagnosticDescriptor ValidatorHasTooManyConstructors =
		new(
			id: DiagnosticIds.IV0009ValidatorHasTooManyConstructors,
			title: "Validator has too many constructors",
			messageFormat: "Validator `{0}` has too many constructors",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Validators with two or more constructors are not yet supported."
		);

	public static readonly DiagnosticDescriptor ValidatorIsMissingDefaultMessage =
		new(
			id: DiagnosticIds.IV0019ValidatorIsMissingDefaultMessage,
			title: "Validator is missing `DefaultMessage`",
			messageFormat: "Validator `{0}` must have a `DefaultMessage` property",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Validators without a `DefaultMessage` will fail when being used."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			ValidateMethodMustExist,
			ValidateMethodMustBeStatic,
			ValidateMethodMustBeUnique,
			ValidateMethodMustReturnValueTuple,
			ValidateMethodIsMissingParameter,
			ValidateMethodHasExtraParameter,
			ValidateMethodParameterIsIncorrectType,
			ValidatePropertyMustBeRequired,
			ValidatorHasTooManyConstructors,
			ValidatorIsMissingDefaultMessage,
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

		var validatorClassSymbol = (INamedTypeSymbol)context.Symbol;

		if (!validatorClassSymbol.BaseType.IsValidatorAttribute())
			return;

		token.ThrowIfCancellationRequested();

		if (!validatorClassSymbol
				.GetMembers()
				.Any(m => m
					is IPropertySymbol
					{
						IsStatic: true,
						Name: "DefaultMessage",
						Type.SpecialType: SpecialType.System_String,
						IsIndexer: false,
						DeclaredAccessibility: Accessibility.Public,
					}
					or IFieldSymbol
					{
						IsConst: true,
						Name: "DefaultMessage",
						Type.SpecialType: SpecialType.System_String,
						DeclaredAccessibility: Accessibility.Public,
					}
				)
		)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					ValidatorIsMissingDefaultMessage,
					validatorClassSymbol.Locations[0],
					validatorClassSymbol.Name)
			);
		}

		token.ThrowIfCancellationRequested();

		if (validatorClassSymbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.Name is "ValidateProperty")
				.ToList()
			is not { Count: > 0 } methods
		)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					ValidateMethodMustExist,
					validatorClassSymbol.Locations[0],
					validatorClassSymbol.Name)
			);

			return;
		}

		token.ThrowIfCancellationRequested();
		if (validatorClassSymbol.Constructors is { Length: > 1 })
		{
			foreach (var constructor in validatorClassSymbol.Constructors)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidatorHasTooManyConstructors,
						constructor.Locations[0],
						validatorClassSymbol.Name)
				);
			}
		}

		token.ThrowIfCancellationRequested();
		switch (methods.Where(m => m.IsStatic).ToList())
		{
			case { Count: var count and not 1 }:
			{
				var diagnostic = count == 0
					? ValidateMethodMustBeStatic
					: ValidateMethodMustBeUnique;

				foreach (var method in methods)
				{
					token.ThrowIfCancellationRequested();

					context.ReportDiagnostic(
						Diagnostic.Create(
							diagnostic,
							method.Locations[0]
						)
					);
				}

				break;
			}

			case [var methodSymbol]:
			{
				CheckValidateMethod(context, validatorClassSymbol, methodSymbol);
				break;
			}

			default:
				// should never happen - all count cases are covered above
				break;
		}
	}

	[SuppressMessage(
		"Globalization",
		"CA1308:Normalize strings to uppercase",
		Justification = "lower/upper case is not done for normalization"
	)]
	private static void CheckValidateMethod(
		SymbolAnalysisContext context,
		INamedTypeSymbol validatorClassSymbol,
		IMethodSymbol methodSymbol
	)
	{
		if (!methodSymbol.ReturnType.IsValidValidatorReturn())
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					ValidateMethodMustReturnValueTuple,
					methodSymbol.Locations[0]
				)
			);
		}

		var parameters = methodSymbol.Parameters.Skip(1).OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase);

		var properties = validatorClassSymbol
			.GetMembers()
			.OfType<IPropertySymbol>()
			.Where(p =>
				p.Name != "Message"
				&& !p.IsStatic
			)
			.Cast<ISymbol>()
			.ToList();

		if (validatorClassSymbol.Constructors is [{ } constructor])
		{
			foreach (var p in constructor.Parameters)
			{
				var i = 0;
				for (; i < properties.Count; i++)
				{
					if (properties[i].Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase))
						break;
				}

				if (i == properties.Count)
					properties.Add(p);
				else
					properties[i] = p;
			}
		}

		properties.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name));

		foreach (var (parameter, property) in parameters.JoinMerge(properties, x => x.Name, x => x.Name, StringComparer.OrdinalIgnoreCase))
		{
			if (parameter is null)
			{
				var paramName = $"{property!.Name[..1].ToLowerInvariant()}{property.Name[1..]}";

				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateMethodIsMissingParameter,
						property.Locations[0],
						property.Name,
						paramName
					)
				);
			}
			else if (property is null)
			{
				var propName = $"{parameter!.Name[..1].ToUpperInvariant()}{parameter.Name[1..]}";

				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidateMethodHasExtraParameter,
						parameter.Locations[0],
						parameter.Name,
						propName
					)
				);
			}
			else
			{
				var (propertyType, isRequired) = property switch
				{
					IParameterSymbol ps => (ps.Type, !ps.HasExplicitDefaultValue),
					IPropertySymbol ps => (ps.Type, ps.IsRequired),
					_ => throw new InvalidOperationException(),
				};

				CheckParameterAllowedType(context, parameter, property, propertyType);
				CheckParameterIsRequired(context, parameter, property, isRequired);
			}
		}
	}

	private static void CheckParameterAllowedType(
		SymbolAnalysisContext context,
		IParameterSymbol parameter,
		ISymbol property,
		ITypeSymbol propertyType
	)
	{
		var parameterType = parameter.Type;

		if (parameter.IsParams
			&& property is IParameterSymbol { IsParams: true }
		)
		{
			propertyType = ((IArrayTypeSymbol)propertyType).ElementType;
			parameterType = ((IArrayTypeSymbol)parameterType).ElementType;
		}

		if (
			propertyType is { SpecialType: SpecialType.System_Object or SpecialType.System_String }
			&& property.GetAttributes().Any(a => a.AttributeClass.IsTargetTypeAttribute())
		)
		{
			return;
		}

		if (SymbolEqualityComparer.IncludeNullability.Equals(propertyType, parameterType))
			return;

		context.ReportDiagnostic(
			Diagnostic.Create(
				ValidateMethodParameterIsIncorrectType,
				parameter.Locations[0],
				parameter.Name,
				property.Name
			)
		);

		context.ReportDiagnostic(
			Diagnostic.Create(
				ValidateMethodParameterIsIncorrectType,
				property.Locations[0],
				parameter.Name,
				property.Name
			)
		);
	}

	private static void CheckParameterIsRequired(SymbolAnalysisContext context, IParameterSymbol parameter, ISymbol property, bool isRequired)
	{
		if (
			!parameter.HasExplicitDefaultValue
			&& !isRequired
		)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					ValidatePropertyMustBeRequired,
					property.Locations[0],
					property.Name
				)
			);
		}
	}
}
