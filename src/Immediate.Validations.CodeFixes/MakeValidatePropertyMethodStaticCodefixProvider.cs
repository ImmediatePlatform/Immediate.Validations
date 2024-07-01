using System.Collections.Immutable;
using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Validations.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public class MakeValidatePropertyMethodStaticCodefixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IV0002ValidateMethodMustBeStatic]);

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
				title: "Make 'ValidateProperty' method static",
				createChangedDocument: _ =>
					MakeValidatePropertyMethodStatic(context.Document, root, methodDeclarationSyntax),
				equivalenceKey: nameof(MakeValidatePropertyMethodStaticCodefixProvider)
			),
			diagnostic);
	}

	private static Task<Document> MakeValidatePropertyMethodStatic(
		Document document,
		CompilationUnitSyntax root,
		MethodDeclarationSyntax methodDeclarationSyntax
	)
	{
		var newMethodDeclaration = methodDeclarationSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

		var newRoot = root.ReplaceNode(methodDeclarationSyntax, newMethodDeclaration);
		var newDocument = document.WithSyntaxRoot(newRoot);
		return Task.FromResult(newDocument);
	}
}

