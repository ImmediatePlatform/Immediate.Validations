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
		// We link only one diagnostic and assume there is only one diagnostic in the context.
		var diagnostic = context.Diagnostics.Single();

		// 'SourceSpan' of 'Location' is the highlighted area. We're going to use this area to find the 'SyntaxNode' to rename.
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

		if (root?.FindNode(diagnosticSpan) is MethodDeclarationSyntax methodDeclarationSyntax &&
		root is CompilationUnitSyntax compilationUnitSyntax)
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "Make 'ValidateProperty' method static",
					createChangedDocument: _ =>
						MakeValidatePropertyMethodStatic(context.Document, compilationUnitSyntax, methodDeclarationSyntax),
					equivalenceKey: nameof(MakeValidatePropertyMethodStaticCodefixProvider)
				),
				diagnostic);
		}
	}

	private static Task<Document> MakeValidatePropertyMethodStatic(
		Document document,
		CompilationUnitSyntax root,
		MethodDeclarationSyntax methodDeclarationSyntax
	)
	{
		var newMethodDeclaration = methodDeclarationSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

		var newRoot = root.ReplaceNode(methodDeclarationSyntax, newMethodDeclaration);

		// Create a new document with the updated syntax root
		var newDocument = document.WithSyntaxRoot(newRoot);

		// Return the new document
		return Task.FromResult(newDocument);
	}
}

