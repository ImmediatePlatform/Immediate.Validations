using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Immediate.Validations.OpenApi;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
#if NET8_0
using Swashbuckle.AspNetCore.Swagger;
#else
using Microsoft.AspNetCore.OpenApi;
#endif

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class OpenApiTests
{
	[Fact]
	public async Task GeneratesBodyAndAsParametersConstraints()
	{
		using var document = await GetDocumentAsync(enableConstraints: true, TestContext.Current.CancellationToken);
		var properties = GetRequestBodyProperties(document.RootElement);
		var parameters = GetParameters(document.RootElement);

		Assert.Equal("string", properties.GetProperty("display_name").GetProperty("type").GetString());
		Assert.Equal(1, properties.GetProperty("quantity").GetProperty("minimum").GetInt32());
		Assert.False(properties.GetProperty("priority").TryGetProperty("minimum", out _));

		var tags = properties.GetProperty("tags");
		Assert.Equal(1, tags.GetProperty("minItems").GetInt32());
		Assert.Equal(12, tags.GetProperty("items").GetProperty("maxLength").GetInt32());

		var id = GetParameter(parameters, "path");
		Assert.Equal("widgetId", id.GetProperty("name").GetString());
		Assert.Equal(2, id.GetProperty("schema").GetProperty("exclusiveMinimum").GetInt32());

		var filter = GetParameter(parameters, "query");
		Assert.Equal("q", filter.GetProperty("name").GetString());
		Assert.Equal(3, filter.GetProperty("schema").GetProperty("minLength").GetInt32());
		Assert.Equal(12, filter.GetProperty("schema").GetProperty("maxLength").GetInt32());
	}

	[Fact]
	public async Task ConstraintsRequireRegistration()
	{
		using var document = await GetDocumentAsync(enableConstraints: false, TestContext.Current.CancellationToken);
		var properties = GetRequestBodyProperties(document.RootElement);
		var parameters = GetParameters(document.RootElement);

		Assert.False(properties.GetProperty("quantity").TryGetProperty("minimum", out _));
		var filter = GetParameter(parameters, "query");
		Assert.False(filter.GetProperty("schema").TryGetProperty("minLength", out _));
	}

	private static async Task<JsonDocument> GetDocumentAsync(bool enableConstraints, CancellationToken cancellationToken)
	{
		var builder = WebApplication.CreateBuilder();
		_ = builder.WebHost.UseTestServer();
		_ = builder.Services.AddEndpointsApiExplorer();
#if NET8_0
		_ = builder.Services.AddSwaggerGen(options =>
#else
		_ = builder.Services.AddOpenApi(options =>
#endif
		{
			if (enableConstraints)
				options.AddImmediateValidations();
		});

		await using var app = builder.Build();
		_ = app.MapPost("/widgets", static (CreateWidget body) => TypedResults.Ok(body.Quantity));
		_ = app.MapGet("/widgets/{widgetId}", static ([AsParameters] FindWidget query) => TypedResults.Ok(query.Id));
		await app.StartAsync(cancellationToken);

#if NET8_0
		var document = app.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("v1");
#else
		var provider = app.Services.GetRequiredKeyedService<IOpenApiDocumentProvider>("v1");
		var document = await provider.GetOpenApiDocumentAsync(cancellationToken);
#endif
		using var buffer = new StringWriter(CultureInfo.InvariantCulture);
		document.SerializeAsV31(new OpenApiJsonWriter(buffer));
		return JsonDocument.Parse(buffer.ToString());
	}

	private static JsonElement GetRequestBodyProperties(JsonElement document)
	{
		var schema = document.GetProperty("paths").GetProperty("/widgets").GetProperty("post")
			.GetProperty("requestBody").GetProperty("content").GetProperty("application/json").GetProperty("schema");

		if (schema.TryGetProperty("$ref", out var reference) && reference.GetString() is { } path)
		{
			const string Prefix = "#/components/schemas/";
			Assert.StartsWith(Prefix, path, StringComparison.Ordinal);
			schema = document.GetProperty("components").GetProperty("schemas")
				.GetProperty(path[Prefix.Length..]);
		}

		return schema.GetProperty("properties");
	}

	private static JsonElement GetParameters(JsonElement document) =>
		document.GetProperty("paths").GetProperty("/widgets/{widgetId}").GetProperty("get").GetProperty("parameters");

	private static JsonElement GetParameter(JsonElement parameters, string location) =>
		Assert.Single(parameters.EnumerateArray(), parameter => string.Equals(parameter.GetProperty("in").GetString(), location, StringComparison.Ordinal));

	[Validate]
	public sealed partial record CreateWidget : IValidationTarget<CreateWidget>
	{
		private const int PriorityFloor = 3;

		[JsonPropertyName("display_name")]
		[NotNull]
		public string? DisplayName { get; init; }

		[GreaterThanOrEqual(1)]
		public int Quantity { get; init; }

		[GreaterThanOrEqual(nameof(PriorityFloor))]
		public int Priority { get; init; }

		[NotEmpty]
		[element: MaxLength(12)]
		public required Collection<string> Tags { get; init; }
	}

	[Validate]
	public sealed partial record FindWidget : IValidationTarget<FindWidget>
	{
		[FromRoute(Name = "widgetId")]
		[GreaterThan(2)]
		public int Id { get; init; }

		[FromQuery(Name = "q")]
		[Length(5, 12)]
		public string? Filter { get; init; }
	}
}
