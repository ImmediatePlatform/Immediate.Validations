using System.Collections.Immutable;
using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Validations.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public class AddValidateMethodCodefixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IV0001ValidateMethodMustExist]);

	public override FixAllProvider? GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		// We link only one diagnostic and assume there is only one diagnostic in the context.
		var diagnostic = context.Diagnostics.Single();

		// 'SourceSpan' of 'Location' is the highlighted area. We're going to use this area to find the 'SyntaxNode' to rename.
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

		if (root is CompilationUnitSyntax compilationUnitSyntax
			&& root.FindNode(diagnosticSpan) is ClassDeclarationSyntax classDeclarationSyntax
		)
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "Add 'ValidateProperty' method",
					createChangedDocument: _ =>
						AddValidatePropertyMethod(context.Document, compilationUnitSyntax, classDeclarationSyntax),
					equivalenceKey: nameof(AddValidateMethodCodefixProvider)
				),
				diagnostic);
		}
	}

	private static Task<Document> AddValidatePropertyMethod(
		Document document,
		CompilationUnitSyntax root,
		ClassDeclarationSyntax classDeclarationSyntax
	)
	{
		var validatePropertyMethod = MethodDeclaration(
				PredefinedType(Token(SyntaxKind.BoolKeyword)),
				Identifier("ValidateProperty"))
			.WithModifiers(
				TokenList(
				[
					Token(SyntaxKind.PublicKeyword),
					Token(SyntaxKind.StaticKeyword),
				]))
			.WithTypeParameterList(
				TypeParameterList(
					SingletonSeparatedList(
						TypeParameter(
							Identifier("T")))))
			.WithParameterList(
				ParameterList(
					SingletonSeparatedList(
						Parameter(
								Identifier("value"))
							.WithType(
								IdentifierName("T")))))
			.WithExpressionBody(
				ArrowExpressionClause(
					LiteralExpression(
						SyntaxKind.DefaultLiteralExpression,
						Token(SyntaxKind.DefaultKeyword))))
			.WithSemicolonToken(
				Token(SyntaxKind.SemicolonToken))
			.WithAdditionalAnnotations(Formatter.Annotation);

		var newMembers = classDeclarationSyntax.Members
			.Insert(
				0,
				validatePropertyMethod
			);

		var newClassDecl = classDeclarationSyntax
			.WithMembers(newMembers);

		// Replace the old class declaration with the new one
		var newRoot = root.ReplaceNode(classDeclarationSyntax, newClassDecl);

		// Create a new document with the updated syntax root
		var newDocument = document.WithSyntaxRoot(newRoot);

		// Return the new document
		return Task.FromResult(newDocument);
	}
}

