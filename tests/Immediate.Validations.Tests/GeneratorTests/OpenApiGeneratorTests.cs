using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
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
		#nullable enable
		using System.Collections.Generic;
		using Immediate.Validations.Shared;

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

	[Fact]
	public async Task OmitsConstraintsWithoutWebReferences()
	{
		var result = GeneratorTestHelper.RunOpenApiGenerators(ValidationSource);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EmitsConstraintsWithAspNetCoreReference()
	{
		var result = GeneratorTestHelper.RunOpenApiGenerators(ValidationSource, Reference(typeof(HttpContext)));

		_ = await Verify(result);
	}

	[Fact]
#if NET8_0
	public async Task GeneratesSwashbuckleAdapter()
#else
	public async Task GeneratesAspNetCoreOpenApiAdapter()
#endif
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
			using Immediate.Validations.OpenApi;

			{{ValidationSource}}

			public static class Registration
			{
				public static {{OptionsType}} Configure({{OptionsType}} options) => options.AddImmediateValidations();
			}
			""",
			references
		);

		_ = await Verify(result);
	}

	private static MetadataReference Reference(Type type) => MetadataReference.CreateFromFile(type.Assembly.Location);
}
