using Microsoft.CodeAnalysis;
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
				(_, _) => true,
				TransformMethod
			)
			.Where(m => m != null);

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
