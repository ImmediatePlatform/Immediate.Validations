using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Validations.CodeFixes;

[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Add AdditionalValidations Method")]
public sealed class AddAdditionalValidationsCodeRefactoringProvider : CodeRefactoringProvider
{
	public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
	{
		var (document, span, token) = context;
		token.ThrowIfCancellationRequested();

		if (await document.GetRequiredSyntaxRootAsync(token) is not CompilationUnitSyntax root)
			return;

		if (root.FindNode(span) is not TypeDeclarationSyntax tds)
			return;

		var model = await document.GetRequiredSemanticModelAsync(token);
		if (model.GetDeclaredSymbol(tds, token) is not INamedTypeSymbol symbol)
			return;

		if (symbol.GetAttributes().Any(a => a.AttributeClass.IsValidateAttribute()) is not true
			|| symbol.HasAdditionalValidationsMethod())
		{
			return;
		}

		context.RegisterRefactoring(
			CodeAction.Create(
				title: "Add AdditionalValidations Method",
				createChangedDocument: _ =>
					AddValidatePropertyMethod(context.Document, root, tds),
				equivalenceKey: nameof(AddAdditionalValidationsCodeRefactoringProvider)
			)
		);
	}

	private static Task<Document> AddValidatePropertyMethod(
		Document document,
		CompilationUnitSyntax root,
		TypeDeclarationSyntax typeDeclarationSyntax
	)
	{
		var validatePropertyMethod = MethodDeclaration(
				PredefinedType(Token(SyntaxKind.VoidKeyword)),
				Identifier("AdditionalValidations"))
			.WithModifiers(
				TokenList(
				[
					Token(SyntaxKind.PrivateKeyword),
					Token(SyntaxKind.StaticKeyword),
				]))
			.WithParameterList(
				ParameterList(
					SeparatedList(
					[
						Parameter(Identifier("errors"))
							.WithType(IdentifierName("ValidationResult")),
						Parameter(Identifier("target"))
							.WithType(IdentifierName(typeDeclarationSyntax.Identifier)),
					])))
			.WithBody(
				Block())
			.WithAdditionalAnnotations(Formatter.Annotation);

		var newClassDecl = typeDeclarationSyntax
			.WithMembers(
				typeDeclarationSyntax.Members
					.Add(validatePropertyMethod)
			);

		var newRoot = root.ReplaceNode(typeDeclarationSyntax, newClassDecl);
		var newDocument = document.WithSyntaxRoot(newRoot);
		return Task.FromResult(newDocument);
	}
}

