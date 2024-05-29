using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class LengthTests
{
	[Validate]
	public partial record StringRecord : IValidationTarget<StringRecord>
	{
		[Length(12)]
		public required string StringValue { get; init; }
	}

	[Fact]
	public void LengthWhenShort()
	{
		var instance = new StringRecord { StringValue = "Hello" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "String is of length '5', which is shorter than the allowed length of '12'.",
				}
			],
			errors
		);
	}

	[Fact]
	public void LengthWhenEqual()
	{
		var instance = new StringRecord { StringValue = "Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void LengthWhenLong()
	{
		var instance = new StringRecord { StringValue = "Hello World! Hello World! Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "String is of length '38', which is longer than the allowed length of '12'.",
				}
			],
			errors
		);
	}
}
