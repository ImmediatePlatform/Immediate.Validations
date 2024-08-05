using System.Globalization;
using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[Collection(nameof(DefaultLocalizerTests))]
[CollectionDefinition(nameof(DefaultLocalizerTests), DisableParallelization = true)]
public sealed partial class DefaultLocalizerTests(DefaultLocalizerTestsFixture fixture) : IClassFixture<DefaultLocalizerTestsFixture>
{
	public DefaultLocalizerTestsFixture Fixture { get; } = fixture;

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
					ErrorMessage = "'Id' must be greater than '0'.",
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
					ErrorMessage = "'Id' doit être supérieur à '0'.",
				},
			],
			errors
		);
	}
}

public sealed class DefaultLocalizerTestsFixture : IDisposable
{
	private readonly CultureInfo _culture;

	public DefaultLocalizerTestsFixture()
	{
		_culture = Thread.CurrentThread.CurrentUICulture;
	}

	public void Dispose()
	{
		Thread.CurrentThread.CurrentUICulture = _culture;
	}
}
