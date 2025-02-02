using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Validations.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedConstructorParameterSuppressor : DiagnosticSuppressor
{
	public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions =>
		ImmutableArray.Create([
			CreateDescriptor("CS9113"),
			CreateDescriptor("CA1019"),
		]);

	public override void ReportSuppressions(SuppressionAnalysisContext context)
	{
		foreach (var diagnostic in context.ReportedDiagnostics)
		{
			context.CancellationToken.ThrowIfCancellationRequested();

			if (diagnostic.Location.SourceTree?.GetRoot().FindNode(diagnostic.Location.SourceSpan) is not { } node)
				continue;

			var model = context.GetSemanticModel(diagnostic.Location.SourceTree);
			var declaration = GetParentTypeDeclaration(node);

			if (declaration is null)
				continue;

			var symbol = (INamedTypeSymbol?)model.GetDeclaredSymbol(declaration);

			if (symbol is null)
				continue;

			if (symbol.BaseType.IsValidatorAttribute())
			{
				var suppression = SupportedSuppressions
					.First(s => string.Equals(s.SuppressedDiagnosticId, diagnostic.Id, StringComparison.Ordinal));

				context.ReportSuppression(
					Suppression.Create(
						suppression,
						diagnostic
					)
				);
			}
		}
	}

	private static TypeDeclarationSyntax? GetParentTypeDeclaration(SyntaxNode? node)
	{
		while (node is not null)
		{
			if (node is TypeDeclarationSyntax typeDeclaration)
				return typeDeclaration;
			node = node.Parent;
		}

		return null;
	}

	private static SuppressionDescriptor CreateDescriptor(string id)
		=> new(
			id: $"{id}Suppression",
			suppressedDiagnosticId: id,
			justification: $"Suppress {id} for Validators."
		);
}
