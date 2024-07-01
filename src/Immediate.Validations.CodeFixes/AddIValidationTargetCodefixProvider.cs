using System.Collections.Immutable;
using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Validations.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class AddIValidationTargetCodefixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IV0013IValidationTargetMissing]);

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
				$"Add `IValidationTarget<{typeDeclaration.Identifier}>`",
				createChangedDocument: _ =>
					AddIValidationTarget(context.Document, root, typeDeclaration),
				equivalenceKey: nameof(AddIValidationTargetCodefixProvider)
			),
			diagnostic
		);
	}

	private static Task<Document> AddIValidationTarget(Document document, CompilationUnitSyntax root, TypeDeclarationSyntax typeDeclaration)
	{
		var newBaseType =
			SimpleBaseType(
				GenericName(
					Identifier("IValidationTarget"),
					TypeArgumentList(
						SingletonSeparatedList<TypeSyntax>(
							IdentifierName(typeDeclaration.Identifier)
						)
					)
				)
			);

		var newDecl = typeDeclaration.BaseList is not null
			? typeDeclaration.AddBaseListTypes(newBaseType)
			: typeDeclaration
				.WithBaseList(
					BaseList(SingletonSeparatedList<BaseTypeSyntax>(newBaseType))
						.WithTrailingTrivia(typeDeclaration.GetTrailingTrivia())
				)
				.WithIdentifier(typeDeclaration.Identifier.WithoutTrivia());

		var newRoot = root.ReplaceNode(typeDeclaration, newDecl);
		var newDocument = document.WithSyntaxRoot(newRoot);
		return Task.FromResult(newDocument);
	}
}
