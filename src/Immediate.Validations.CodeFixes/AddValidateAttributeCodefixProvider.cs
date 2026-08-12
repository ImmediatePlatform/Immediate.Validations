using System.Collections.Immutable;
using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
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
				createChangedDocument: token =>
					AddValidateAttribute(context.Document, root, typeDeclaration, token),
				equivalenceKey: nameof(AddValidateAttributeCodefixProvider)
			),
			diagnostic
		);
	}

	private static async Task<Document> AddValidateAttribute(Document document, CompilationUnitSyntax root, TypeDeclarationSyntax typeDeclaration, CancellationToken token)
	{
		var model = await document.GetSemanticModelAsync(token);

		var validateSymbol = model?.Compilation
			.GetTypeByMetadataName("Immediate.Validations.Shared.ValidateAttribute")!;

		var referenceId = DocumentationCommentId.CreateReferenceId(validateSymbol);
		var annotation = new SyntaxAnnotation("SymbolId", referenceId);

		var newLineSyntax = typeDeclaration.DescendantTrivia()
			.FirstOrDefault(t => t.IsKind(SyntaxKind.EndOfLineTrivia));

		if (newLineSyntax == default)
			newLineSyntax = ElasticLineFeed;

		var validateAttribute = AttributeList(
			SingletonSeparatedList(
				Attribute(
					IdentifierName("Validate")
				)
			)
		)
			.WithTrailingTrivia(newLineSyntax);

		var newDecl = typeDeclaration.AttributeLists switch
		{
			[] =>
				typeDeclaration
					.WithoutLeadingTrivia()
					.WithAttributeLists(
						typeDeclaration.AttributeLists
							.Add(validateAttribute.WithLeadingTrivia(typeDeclaration.GetLeadingTrivia()))
					),

			_ =>
				typeDeclaration
					.WithAttributeLists(
						typeDeclaration.AttributeLists
							.Add(validateAttribute)
					),
		};

		var newRoot = root.ReplaceNode(
			typeDeclaration,
			newDecl
				.WithAdditionalAnnotations(Simplifier.AddImportsAnnotation, annotation)
				.WithAdditionalAnnotations(Formatter.Annotation)
		);

		return document.WithSyntaxRoot(newRoot);
	}
}
