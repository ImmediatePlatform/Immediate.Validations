using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Immediate.Validations.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AssemblyBehaviorAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor AssemblyBehaviorsShouldUseValidation =
		new(
			id: DiagnosticIds.IV0011AssemblyBehaviorsShouldUseValidation,
			title: "Assembly-wide `Behaviors` attribute should use `ValidationBehavior<,>`",
			messageFormat: "Assembly-wide `Behaviors` attribute should use `ValidationBehavior<,>`",
			category: "ImmediateValidations",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "`ValidationBehavior<,>` must be referenced in `Behaviors` type list for validation to work in Handlers."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create<DiagnosticDescriptor>(
		[
			AssemblyBehaviorsShouldUseValidation,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeOperation, OperationKind.Attribute);
	}

	private static void AnalyzeOperation(OperationAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		if (context is not
			{
				Operation: IAttributeOperation { Operation: IObjectCreationOperation attribute },
				ContainingSymbol: ISymbol targetSymbol,
			})
		{
			return;
		}

		if (!attribute.Type.IsBehaviorsAttribute())
			return;

		if (attribute.Arguments.Length != 1)
		{
			// note: this will already be a compiler error anyway
			return;
		}

		if (targetSymbol is not INamespaceSymbol { IsGlobalNamespace: true })
			return;

		token.ThrowIfCancellationRequested();
		var array = attribute.Arguments[0].Value;

		if (array is not
			{
				Type: IArrayTypeSymbol
				{
					ElementType:
					{
						Name: "Type",
						ContainingNamespace:
						{
							Name: "System",
							ContainingNamespace.IsGlobalNamespace: true,
						},
					},
				},
				ChildOperations.Count: 2
			}
			|| array.ChildOperations.ElementAt(1) is not IArrayInitializerOperation aio)
		{
			// note: this will already be a compiler error anyway
			return;
		}

		token.ThrowIfCancellationRequested();

		foreach (var op in aio.ChildOperations)
		{
			token.ThrowIfCancellationRequested();
			if (op is not ITypeOfOperation
				{
					TypeOperand: INamedTypeSymbol behaviorType,
				}
			)
			{
				continue;
			}

			if (behaviorType.IsValidationBehavior())
				return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				AssemblyBehaviorsShouldUseValidation,
				attribute.Syntax.GetLocation()
			)
		);
	}
}
