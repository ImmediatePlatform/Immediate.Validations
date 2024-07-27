using System.Globalization;
using Immediate.Validations.Shared;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class LocalizationTests : IDisposable
{
	private readonly NullLoggerFactory _loggerFactory;

	public LocalizationTests()
	{
		var resourceManager = Resources.Validators.ResourceManager;
		var assembly = typeof(LocalizationTests).Assembly;
		var cache = new ResourceNamesCache();
		_loggerFactory = new NullLoggerFactory();
		var logger = new Logger<LocalizationTests>(_loggerFactory);
		ValidationConfiguration.Localizer = new ResourceManagerStringLocalizer(resourceManager, assembly, resourceManager.BaseName, cache, logger);
		var t = ValidationConfiguration.Localizer["GreaterThanAttribute"];
		var _ = t.Value;
	}

	[Validate]
	public sealed partial record ValidateRecord : IValidationTarget<ValidateRecord>
	{
		[GreaterThan(0)]
		public required int Id { get; init; }
	}

	[Fact]
	public void DefaultLocalizedMessage()
	{
		var record = new ValidateRecord { Id = 0 };

		Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");

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

	public void Dispose()
	{
		_loggerFactory.Dispose();
	}
}
