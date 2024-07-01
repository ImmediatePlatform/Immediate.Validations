using System.Collections.Immutable;
using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Validations.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class AddValidateAttributeCodefixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IV0012ValidateAttributeMissing]);

	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var diagnostic = context.Diagnostics.Single();
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not CompilationUnitSyntax root)
			return;

		if (root.FindNode(diagnosticSpan) is not TypeDeclarationSyntax typeDeclaration)
			return;

		context.RegisterCodeFix(
			CodeAction.Create(
				"Add `[Validate]`",
				createChangedDocument: _ =>
					AddValidateAttribute(context.Document, root, typeDeclaration),
				equivalenceKey: typeDeclaration.Identifier.ToString()
			),
			diagnostic
		);
	}

	private static Task<Document> AddValidateAttribute(Document document, CompilationUnitSyntax root, TypeDeclarationSyntax typeDeclaration)
	{
		var newDecl = typeDeclaration
			.WithAttributeLists(
				typeDeclaration.AttributeLists
					.Add(AttributeList(
						SingletonSeparatedList(
							Attribute(
								IdentifierName("Validate"))))));

		var newRoot = root.ReplaceNode(typeDeclaration, newDecl);
		var newDocument = document.WithSyntaxRoot(newRoot);
		return Task.FromResult(newDocument);
	}
}
