using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class LengthTests
{
	[Validate]
	public partial record StringRecord : IValidationTarget<StringRecord>
	{
		[Length(12, 12)]
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
					ErrorMessage = "'String Value' must be between 12 and 12 characters.",
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
					ErrorMessage = "'String Value' must be between 12 and 12 characters.",
				}
			],
			errors
		);
	}
}
