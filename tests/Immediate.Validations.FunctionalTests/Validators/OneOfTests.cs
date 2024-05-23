using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class OneOfTests
{
	[Validate]
	public partial record StringRecord : IValidationTarget<StringRecord>
	{
		[OneOf("123", "456", "789")]
		public required string StringValue { get; init; }
	}

	[Fact]
	public void StringValueIsOneOfNoErrors()
	{
		var instance = new StringRecord { StringValue = "123" };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void StringValueIsNotOneOfHasErrors()
	{
		var instance = new StringRecord { StringValue = "124" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "Value `124` was not one of the specified values: 123, 456, 789.",
				}
			],
			errors
		);
	}
}
