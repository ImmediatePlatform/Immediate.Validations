using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class MaxLengthTests
{
	[Validate]
	public partial record StringRecord : IValidationTarget<StringRecord>
	{
		[MaxLength(30)]
		public required string StringValue { get; init; }
	}

	[Fact]
	public void MaxLengthWhenShort()
	{
		var instance = new StringRecord { StringValue = "Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void MaxLengthWhenLong()
	{
		var instance = new StringRecord { StringValue = "Hello World! Hello World! Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "String is of length '38', which is longer than the maximum allowed length of '30'.",
				}
			],
			errors
		);
	}
}
