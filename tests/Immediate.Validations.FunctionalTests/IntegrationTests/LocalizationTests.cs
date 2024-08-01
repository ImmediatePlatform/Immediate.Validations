using System.Globalization;
using Immediate.Validations.FunctionalTests.Common;
using Immediate.Validations.Shared;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[Collection(nameof(IsolatedTestCollectionDefinition))]
public sealed partial class LocalizationTests
{
	[Validate]
	public sealed partial record ValidateRecord : IValidationTarget<ValidateRecord>
	{
		[GreaterThan(0)]
		public required int Id { get; init; }
	}

	[Fact]
	public void DefaultLocalizedMessage()
	{
		using var loggerFactory = new NullLoggerFactory();
		var resourceManagerLocalizer = CreateResourceManagerLocalizer(loggerFactory);
		var defaultLocalizer = ValidationConfiguration.Localizer;

		ValidationConfiguration.Localizer = resourceManagerLocalizer;

		var record = new ValidateRecord { Id = 0 };

		Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");

		var errors = ValidateRecord.Validate(record);

		ValidationConfiguration.Localizer = defaultLocalizer;

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

	private static ResourceManagerStringLocalizer CreateResourceManagerLocalizer(ILoggerFactory loggerFactory)
	{
		var resourceManager = Resources.Validators.ResourceManager;
		var assembly = typeof(LocalizationTests).Assembly;
		var cache = new ResourceNamesCache();
		var logger = new Logger<LocalizationTests>(loggerFactory);
		return new ResourceManagerStringLocalizer(resourceManager, assembly, resourceManager.BaseName, cache, logger);
	}
}
