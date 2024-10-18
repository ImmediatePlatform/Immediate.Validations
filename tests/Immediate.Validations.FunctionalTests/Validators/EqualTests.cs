using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class EqualTests
{
	[Validate]
	public partial record IntEqualRecord : IValidationTarget<IntEqualRecord>
	{
		[Equal(0)]
		public required int IntValue { get; init; }
	}

	[Validate]
	public partial record StringEqualRecord : IValidationTarget<StringEqualRecord>
	{
		[Equal(nameof(KeyValue))]
		public required string StringValue { get; init; }
		public required string KeyValue { get; init; }
	}

	[Test]
	public void IntEqualRecordWhenZero()
	{
		var instance = new IntEqualRecord { IntValue = 0 };

		var errors = IntEqualRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void IntEqualRecordWhenNotZero()
	{
		var instance = new IntEqualRecord { IntValue = 1 };

		var errors = IntEqualRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntEqualRecord.IntValue),
					ErrorMessage = "'Int Value' must be equal to '0'.",
				}
			],
			errors
		);
	}

	[Test]
	public void StringEqualRecordWhenEqual()
	{
		var instance = new StringEqualRecord { StringValue = "Test", KeyValue = "Test" };

		var errors = StringEqualRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void StringEqualRecordWhenNotEqual()
	{
		var instance = new StringEqualRecord { StringValue = "asdf", KeyValue = "Test" };

		var errors = StringEqualRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringEqualRecord.StringValue),
					ErrorMessage = "'String Value' must be equal to 'Test'.",
				}
			],
			errors
		);
	}
}
