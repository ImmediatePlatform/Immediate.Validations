using System.Globalization;
using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[NotInParallel]
public sealed partial class DefaultLocalizerTests
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

	[Test]
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
