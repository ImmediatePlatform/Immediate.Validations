using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi;
#if NET8_0
using Swashbuckle.AspNetCore.SwaggerGen;
#else
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
#endif

namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class OpenApiGeneratorTests
{
	private const string ValidationSource = """
		[Validate]
		public sealed partial record Request(
			[property: NotNull, Length(5, 12)] string? Name,
			[property: GreaterThan(2)] int Id
		) : IValidationTarget<Request>
		{
			[LessThan(nameof(Id))]
			public int Limit { get; init; }

			[element: MaxLength(8)]
			public required List<List<string?>> Tags { get; init; }
		}
		""";

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public void EmitsConstraintsOnlyWithWebReferences(bool useAspNetCore)
	{
		var result = GeneratorTestHelper.RunOpenApiGenerators(
			$$"""
			#nullable enable
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			{{ValidationSource}}
			""",
			useAspNetCore ? [Reference(typeof(HttpContext))] : []
		);

		var tree = Assert.Single(result.GeneratedTrees);
		var constraints = tree.GetRoot(TestContext.Current.CancellationToken)
			.DescendantNodes()
			.OfType<AttributeSyntax>()
			.Where(attribute => attribute.Name.GetLastToken().ValueText is "OpenApiConstraintAttribute")
			.Select(attribute =>
			{
				var arguments = attribute.ArgumentList!.Arguments;
				return (
					arguments[0].Expression.GetFirstToken().ValueText,
					arguments[1].Expression.GetLastToken().ValueText,
					arguments[2].Expression.GetFirstToken().ValueText,
					arguments[3].Expression.GetFirstToken().ValueText
				);
			});

		if (!useAspNetCore)
		{
			Assert.Empty(constraints);
			return;
		}

		Assert.Equal(
			[
				("Name", "NotNull", "null", "0"),
				("Name", "MinLength", "3", "0"),
				("Name", "MaxLength", "12", "0"),
				("Id", "ExclusiveMinimum", "2", "0"),
				("Tags", "NotNull", "null", "0"),
				("Tags", "NotNull", "null", "1"),
				("Tags", "MaxLength", "8", "2"),
			],
			constraints
		);
	}

	[Fact]
	public void CompilesAdapterForReferencedProvider()
	{
#if NET8_0
		const string OptionsType = "Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions";
#else
		const string OptionsType = "Microsoft.AspNetCore.OpenApi.OpenApiOptions";
#endif
		Type[] referenceTypes =
		[
			typeof(HttpContext),
			typeof(OpenApiSchema),
#if NET8_0
			typeof(SwaggerGenOptions),
#else
			typeof(OpenApiOptions),
			typeof(ApiParameterDescription),
			typeof(IParameterInfoParameterDescriptor),
			typeof(ModelMetadata),
#endif
		];
		var references = referenceTypes.DistinctBy(type => type.Assembly).Select(Reference).ToArray();

		var result = GeneratorTestHelper.RunOpenApiGenerators(
			$$"""
			#nullable enable
			using System.Collections.Generic;
			using Immediate.Validations.OpenApi;
			using Immediate.Validations.Shared;

			public static class Registration
			{
				public static {{OptionsType}} Configure({{OptionsType}} options) => options.AddImmediateValidations();
			}

			{{ValidationSource}}
			""",
			references
		);

		var adapter = Assert.Single(result.Results[1].GeneratedSources);
		Assert.Equal("IV.OpenApiValidation.g.cs", adapter.HintName);
	}

	private static MetadataReference Reference(Type type) => MetadataReference.CreateFromFile(type.Assembly.Location);
}
