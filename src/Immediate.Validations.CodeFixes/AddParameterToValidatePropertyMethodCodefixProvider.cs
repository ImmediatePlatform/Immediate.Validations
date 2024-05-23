using System.Collections.Immutable;
using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Validations.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public class AddParameterToValidatePropertyMethodCodefixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IV0005ValidateMethodIsMissingParameter]);

	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		// We link only one diagnostic and assume there is only one diagnostic in the context.
		var diagnostic = context.Diagnostics.Single();

		// 'SourceSpan' of 'Location' is the highlighted area. We're going to use this area to find the 'SyntaxNode' to rename.
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

		if (root is CompilationUnitSyntax compilationUnitSyntax)
		{
			if (root.FindNode(diagnosticSpan) is PropertyDeclarationSyntax propertyDeclarationSyntax)
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "Add parameter to 'ValidateProperty' method",
						createChangedDocument: _ =>
							AddParameterToValidatePropertyMethodFromPropertySyntax(context.Document, compilationUnitSyntax, propertyDeclarationSyntax),
						equivalenceKey: nameof(AddParameterToValidatePropertyMethodCodefixProvider)
					),
					diagnostic);
			}

			if (root.FindNode(diagnosticSpan) is ParameterSyntax parameterSyntax
			)
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						title: "Add parameter to 'ValidateProperty' method",
						createChangedDocument: _ =>
							AddParameterToValidatePropertyMethodFromParameterSyntax(context.Document, compilationUnitSyntax, parameterSyntax),
						equivalenceKey: nameof(AddParameterToValidatePropertyMethodCodefixProvider)
					),
					diagnostic);
			}
		}
	}

	private static Task<Document> AddParameterToValidatePropertyMethodFromPropertySyntax(
		Document document,
		CompilationUnitSyntax root,
		PropertyDeclarationSyntax propertyDeclarationSyntax
	)
	{
		if (propertyDeclarationSyntax.Parent is not ClassDeclarationSyntax classDeclarationSyntax)
			throw new InvalidOperationException("Class declaration not found");

		var methodDeclaration = classDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.Text == "ValidateProperty") ?? throw new InvalidOperationException("ValidateProperty method not found");

		var newMethodDeclaration = methodDeclaration.AddParameterListParameters(
			Parameter(Identifier(StringHelpers.ToCamelCase(propertyDeclarationSyntax.Identifier.Text))).WithType(propertyDeclarationSyntax.Type)
		);

		var newRoot = root.ReplaceNode(methodDeclaration, newMethodDeclaration);

		// Create a new document with the updated syntax root
		var newDocument = document.WithSyntaxRoot(newRoot);

		// Return the new document
		return Task.FromResult(newDocument);
	}

	private static Task<Document> AddParameterToValidatePropertyMethodFromParameterSyntax(
		Document document,
		CompilationUnitSyntax root,
		ParameterSyntax parameterSyntax
	)
	{
		ClassDeclarationSyntax classDeclarationSyntax;

#pragma warning disable IDE0045
		if (parameterSyntax.Parent?.Parent is ClassDeclarationSyntax primaryCtorClassDeclSyntax)
			classDeclarationSyntax = primaryCtorClassDeclSyntax;
		else if (parameterSyntax.Parent?.Parent is ConstructorDeclarationSyntax constructorDeclarationSyntax)
			classDeclarationSyntax = (ClassDeclarationSyntax)constructorDeclarationSyntax.Parent!;
		else
			throw new InvalidOperationException("Class declaration not found");
#pragma warning restore IDE0045

		var methodDeclaration = classDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.Text == "ValidateProperty") ?? throw new InvalidOperationException("ValidateProperty method not found");

		var newMethodDeclaration = methodDeclaration.AddParameterListParameters(
			Parameter(Identifier(StringHelpers.ToCamelCase(parameterSyntax.Identifier.Text))).WithType(parameterSyntax.Type)
		);

		var newRoot = root.ReplaceNode(methodDeclaration, newMethodDeclaration);

		// Create a new document with the updated syntax root
		var newDocument = document.WithSyntaxRoot(newRoot);

		// Return the new document
		return Task.FromResult(newDocument);
	}
}

