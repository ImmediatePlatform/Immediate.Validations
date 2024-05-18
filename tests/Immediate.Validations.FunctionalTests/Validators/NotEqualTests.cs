using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class NotEqualTests
{
	[Validate]
	public partial record IntEqualRecord : IValidationTarget<IntEqualRecord>
	{
		[NotEqual(0)]
		public required int IntValue { get; init; }
	}

	[Validate]
	public partial record StringEqualRecord : IValidationTarget<StringEqualRecord>
	{
		[NotEqual(nameof(KeyValue))]
		public required string StringValue { get; init; }
		public required string KeyValue { get; init; }
	}

	[Fact]
	public void IntEqualRecordWhenZero()
	{
		var instance = new IntEqualRecord { IntValue = 1 };

		var errors = IntEqualRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void IntEqualRecordWhenNotZero()
	{
		var instance = new IntEqualRecord { IntValue = 0 };

		var errors = IntEqualRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntEqualRecord.IntValue),
					ErrorMessage = "Value '0' is equal to '0'",
				}
			],
			errors
		);
	}

	[Fact]
	public void StringEqualRecordWhenEqual()
	{
		var instance = new StringEqualRecord { StringValue = "asdf", KeyValue = "Test" };

		var errors = StringEqualRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void StringEqualRecordWhenNotEqual()
	{
		var instance = new StringEqualRecord { StringValue = "Test", KeyValue = "Test" };

		var errors = StringEqualRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringEqualRecord.StringValue),
					ErrorMessage = "Value 'Test' is equal to 'Test'",
				}
			],
			errors
		);
	}
}
