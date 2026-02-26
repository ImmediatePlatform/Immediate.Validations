using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;

namespace Immediate.Validations.Generators;

[Generator]
public sealed partial class ImmediateValidationsGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var validations = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Validations.Shared.ValidateAttribute",
				predicate: (node, _) => node is TypeDeclarationSyntax,
				transform: (ctx, ct) => new ValidateTargetTransformer(ctx, ct).Transform()
			)
			.Where(m => m != null)
			.WithTrackingName("ValidationClasses");

		var template = Utility.GetTemplate("Validations");
		context.RegisterSourceOutput(
			validations,
			(spc, v) => RenderValidation(spc, v!, template)
		);
	}

	private static void RenderValidation(SourceProductionContext context, ValidationTarget v, Template template)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var source = template.Render(v);
		token.ThrowIfCancellationRequested();

		var name = $"{v.Namespace}.{string.Join(".", v.OuterClasses.Select(c => c.Name))}.{v.Class.Name}";
		context.AddSource($"IV.{name}.g.cs", source);
	}
}
