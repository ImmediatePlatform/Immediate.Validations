using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Validations.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidAttributeTargetSuppressor : DiagnosticSuppressor
{
	public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions =>
		ImmutableArray.Create([
			new SuppressionDescriptor(
				id: "InvalidAttributeTargetSuppression",
				suppressedDiagnosticId: "CS0658",
				justification: "Suppress invalid attribute target when used for validation."
			),
		]);

	public override void ReportSuppressions(SuppressionAnalysisContext context)
	{
		var token = context.CancellationToken;

		foreach (var diagnostic in context.ReportedDiagnostics)
		{
			token.ThrowIfCancellationRequested();

			var syntaxTree = diagnostic.Location.SourceTree;

			if (syntaxTree
					?.GetRoot(token)
					.FindNode(diagnostic.Location.SourceSpan) is not AttributeTargetSpecifierSyntax
					{
						Identifier.ValueText: "element",
						Parent.Parent: SyntaxNode declarationSyntax,
					})
			{
				continue;
			}

			if (context.GetSemanticModel(syntaxTree).GetDeclaredSymbol(declarationSyntax, token) switch
			{
				IPropertySymbol
				{
					ContainingType: INamedTypeSymbol ct1,
					Type: ITypeSymbol pt1,
				} => (true, ct1, pt1),

				IParameterSymbol
				{
					ContainingType: INamedTypeSymbol ct1,
					Type: ITypeSymbol pt1,
				} => (true, ct1, pt1),

				_ => (false, null, null),
			} is not (true, { } containerSymbol, { } propertyTypeSymbol))
			{
				continue;
			}

			if (!containerSymbol.GetAttributes().Any(a => a.AttributeClass.IsValidateAttribute()))
				continue;

			var isCollection = propertyTypeSymbol switch
			{
				IArrayTypeSymbol => true,

				INamedTypeSymbol
				{
					IsGenericType: true,
					TypeArguments: [{ }],
				} nts when
					nts.IsICollection1()
					|| nts.IsIReadOnlyCollection1()
					|| nts.AllInterfaces.Any(i => i.IsICollection1() || i.IsIReadOnlyCollection1()) =>
					true,

				_ => false,
			};

			if (!isCollection)
				continue;

			context.ReportSuppression(
				Suppression.Create(
					SupportedSuppressions[0],
					diagnostic
				)
			);

			break;
		}
	}
}
