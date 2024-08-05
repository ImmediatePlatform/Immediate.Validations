using System.Globalization;
using Immediate.Validations.Shared;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[Collection(nameof(CustomLocalizerTests))]
[CollectionDefinition(nameof(CustomLocalizerTests), DisableParallelization = true)]
public sealed partial class CustomLocalizerTests(CustomLocalizerTestsFixture fixture) : IClassFixture<CustomLocalizerTestsFixture>
{
	public CustomLocalizerTestsFixture Fixture { get; } = fixture;

	[Validate]
	public sealed partial record ValidateRecord : IValidationTarget<ValidateRecord>
	{
		[GreaterThan(0)]
		public required int Id { get; init; }
	}

	[Fact]
	public void EnLocalizedMessage()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

		var record = new ValidateRecord { Id = 0 };

		var errors = ValidateRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Id",
					ErrorMessage = "'Id' must not be empty.",
				},
			],
			errors
		);
	}

	[Fact]
	public void FrLocalizedMessage()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");

		var record = new ValidateRecord { Id = 0 };

		var errors = ValidateRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Id",
					ErrorMessage = "'Id' ne doit pas être vide.",
				},
			],
			errors
		);
	}
}

public sealed class CustomLocalizerTestsFixture : IDisposable
{
	private static IStringLocalizer? s_defaultLocalizer;
	private readonly CultureInfo _culture;

	public CustomLocalizerTestsFixture()
	{
		s_defaultLocalizer ??= ValidationConfiguration.Localizer;
		_culture = Thread.CurrentThread.CurrentUICulture;

		ValidationConfiguration.Localizer = CreateResourceManagerLocalizer();
	}

	public void Dispose()
	{
		Thread.CurrentThread.CurrentUICulture = _culture;

		if (s_defaultLocalizer != null)
			ValidationConfiguration.Localizer = s_defaultLocalizer;
	}

	private static ResourceManagerStringLocalizer CreateResourceManagerLocalizer()
	{
		var resourceManager = Resources.Validators.ResourceManager;
		var assembly = typeof(CustomLocalizerTests).Assembly;
		var cache = new ResourceNamesCache();
		var logger = NullLogger<CustomLocalizerTests>.Instance;
		return new ResourceManagerStringLocalizer(resourceManager, assembly, resourceManager.BaseName, cache, logger);
	}
}
