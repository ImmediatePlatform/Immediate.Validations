using Immediate.Validations.Shared;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[NotInParallel]
public sealed partial class CustomLocalizerTests
{
	[Validate]
	public sealed partial record ValidateRecord : IValidationTarget<ValidateRecord>
	{
		[GreaterThan(0)]
		public required int Id { get; init; }
	}

	[Test]
	public void EnLocalizedMessage()
	{
		using var scope = new LocalizerScope("en-US");

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

	[Test]
	public void FrLocalizedMessage()
	{
		using var scope = new LocalizerScope("fr-CA");

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

public sealed class LocalizerScope : IDisposable
{
	private readonly IStringLocalizer? _defaultLocalizer;
	private readonly CultureScope _cultureScope;

	public LocalizerScope(string culture)
	{
		_defaultLocalizer ??= ValidationConfiguration.Localizer;
		_cultureScope = new(culture);

		ValidationConfiguration.Localizer = CreateResourceManagerLocalizer();
	}

	public void Dispose()
	{
		_cultureScope.Dispose();

		if (_defaultLocalizer != null)
			ValidationConfiguration.Localizer = _defaultLocalizer;
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
