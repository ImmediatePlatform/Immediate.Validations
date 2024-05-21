using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class AdditionalValidationsTests
{
	[Validate]
	public sealed partial record ValidateRecord : IValidationTarget<ValidateRecord>
	{
		public required int Id { get; init; }

		private static IEnumerable<ValidationError> AdditionalValidations(ValidateRecord target)
		{
			if (target.Id % 2 == 1)
			{
				yield return new()
				{
					PropertyName = "Id",
					ErrorMessage = "Value is not even.",
				};
			}
		}
	}

	[Fact]
	public void EvenIdNoErrors()
	{
		var record = new ValidateRecord { Id = 0 };

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
}
