using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class AdditionalValidationsTests
{
	[Validate]
	public sealed partial record ValidateRecord : IValidationTarget<ValidateRecord>
	{
		public required int Id { get; init; }

		private static void AdditionalValidations(ValidationResult errors, ValidateRecord target)
		{
			errors.Add(
				() => NotEmptyAttribute.ValidateProperty(
					target.Id
				)
			);
			if (target.Id % 2 == 1)
			{
				errors.Add(new()
				{
					PropertyName = "Id",
					ErrorMessage = "Value is not even.",
				});
			}
		}
	}

	[Fact]
	public void EvenIdNoErrors()
	{
		var record = new ValidateRecord { Id = 2 };

		var errors = ValidateRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void OddIdWithErrors()
	{
		var record = new ValidateRecord { Id = 1 };

		var errors = ValidateRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Id",
					ErrorMessage = "Value is not even.",
				},
			],
			errors
		);
	}

	[Fact]
	public void ZeroWithErrors()
	{
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
}
