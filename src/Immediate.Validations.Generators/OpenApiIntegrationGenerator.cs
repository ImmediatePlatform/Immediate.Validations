using Microsoft.CodeAnalysis;

namespace Immediate.Validations.Generators;

[Generator]
public sealed class OpenApiIntegrationGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var providers = context.CompilationProvider
			.Select(static (compilation, _) => DetectProviders(compilation))
			.WithTrackingName("OpenApiProviders");
		var template = Utility.GetTemplate("OpenApiValidation");

		context.RegisterSourceOutput(
			providers,
			(spc, integration) =>
			{
				if (integration.BuiltIn || integration.Swashbuckle)
					spc.AddSource("IV.OpenApiValidation.g.cs", template.Render(integration));
			}
		);
	}

	private static Providers DetectProviders(Compilation compilation)
	{
		if (compilation.GetTypeByMetadataName("Immediate.Validations.Shared.OpenApiConstraintAttribute") is null
			|| compilation.GetTypeByMetadataName("Microsoft.OpenApi.OpenApiSchema") is not { } schema
			|| compilation.GetTypeByMetadataName("Microsoft.OpenApi.IOpenApiSchema") is not { } schemaInterface)
		{
			return default;
		}

		return new(
			BuiltIn: HasSchemaParameter(
				compilation.GetTypeByMetadataName("Microsoft.AspNetCore.OpenApi.IOpenApiSchemaTransformer"),
				"TransformAsync",
				schema
			),
			Swashbuckle: HasSchemaParameter(
				compilation.GetTypeByMetadataName("Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter"),
				"Apply",
				schemaInterface
			)
		);
	}

	private static bool HasSchemaParameter(INamedTypeSymbol? provider, string methodName, INamedTypeSymbol schema) =>
		provider?.GetMembers(methodName).OfType<IMethodSymbol>().Any(
			m => m.Parameters is [{ Type: { } type }, ..] && SymbolEqualityComparer.Default.Equals(type, schema)
		) is true;

	private readonly record struct Providers(bool BuiltIn, bool Swashbuckle);
}
