using System.Collections.Immutable;
using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Validations.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class CorrectValidatePropertyReturnTypeCodefixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IV0004ValidateMethodMustReturnValueTuple]);

	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var diagnostic = context.Diagnostics.Single();
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not CompilationUnitSyntax root)
			return;

		if (root.FindNode(diagnosticSpan) is not MethodDeclarationSyntax methodDeclarationSyntax)
			return;

		context.RegisterCodeFix(
			CodeAction.Create(
				title: "Correct 'ValidateProperty' return type",
				createChangedDocument: _ =>
					CorrectValidatePropertyReturnType(context.Document, root, methodDeclarationSyntax),
				equivalenceKey: nameof(CorrectValidatePropertyReturnType)
			),
			diagnostic
		);
	}

	private static Task<Document> CorrectValidatePropertyReturnType(
		Document document,
		CompilationUnitSyntax root,
		MethodDeclarationSyntax methodDeclarationSyntax
	)
	{
		var newMethodDeclarationSyntax = methodDeclarationSyntax
			.WithReturnType(PredefinedType(Token(SyntaxKind.BoolKeyword)));

		var newRoot = root.ReplaceNode(methodDeclarationSyntax, newMethodDeclarationSyntax);
		var newDocument = document.WithSyntaxRoot(newRoot);
		return Task.FromResult(newDocument);
	}
}

