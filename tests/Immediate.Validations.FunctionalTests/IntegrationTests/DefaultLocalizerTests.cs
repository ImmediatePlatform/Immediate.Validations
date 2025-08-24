using System.Globalization;
using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[CollectionDefinition("DefaultLocalizerTests", DisableParallelization = true)]
public sealed class DefaultLocalizer : ICollectionFixture<DefaultLocalizer>;

[Collection("DefaultLocalizerTests")]
public sealed partial class DefaultLocalizerTests
{
	[Validate]
	public sealed partial record ValidateRecord : IValidationTarget<ValidateRecord>
	{
		[GreaterThan(0)]
		public required int Id { get; init; }
	}

	[Validate]
	public sealed partial record NotNullRecord : IValidationTarget<NotNullRecord>
	{
		public required string Value { get; init; }
	}

	[Fact]
	public void EnLocalizedMessage()
	{
		using var scope = new CultureScope("en-US");

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
		using var scope = new CultureScope("fr-CA");

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

	[Fact]
	public void EnLocalizedNotNullMessage()
	{
		using var scope = new CultureScope("en-US");

		var record = new NotNullRecord { Value = null! };

		var errors = NotNullRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value",
					ErrorMessage =  "'Value' must not be null.",
				},
			],
			errors
		);
	}

	[Fact]
	public void FrLocalizedNotNullMessage()
	{
		using var scope = new CultureScope("fr-CA");

		var record = new NotNullRecord { Value = null! };

		var errors = NotNullRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Value",
					ErrorMessage =  "'Value' ne doit pas être nul.",
				},
			],
			errors
		);
	}
}

public sealed class CultureScope : IDisposable
{
	private readonly CultureInfo _culture;

	public CultureScope(string culture)
	{
		_culture = Thread.CurrentThread.CurrentUICulture;
		Thread.CurrentThread.CurrentUICulture = new(culture);
	}

	public void Dispose()
	{
		Thread.CurrentThread.CurrentUICulture = _culture;
	}
}
