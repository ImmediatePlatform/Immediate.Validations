using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class MinLengthTests
{
	[Validate]
	public partial record StringRecord : IValidationTarget<StringRecord>
	{
		[MinLength(30)]
		public required string StringValue { get; init; }
	}

	[Fact]
	public void MinLengthWhenShort()
	{
		var instance = new StringRecord { StringValue = "Hello World! Hello World! Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void MinLengthWhenLong()
	{
		var instance = new StringRecord { StringValue = "Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "'String Value' must be more than 30 characters.",
				}
			],
			errors
		);
	}
}
