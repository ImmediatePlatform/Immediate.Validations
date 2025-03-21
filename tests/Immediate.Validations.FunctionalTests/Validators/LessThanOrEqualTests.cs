using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class LessThanOrEqualTests
{
	[Validate]
	public partial record IntLessThanRecord : IValidationTarget<IntLessThanRecord>
	{
		[LessThanOrEqual(0)]
		public required int IntValue { get; init; }
	}

	[Validate]
	public partial record StringLessThanRecord : IValidationTarget<StringLessThanRecord>
	{
		[LessThanOrEqual(nameof(KeyValue))]
		public required string StringValue { get; init; }
		public required string KeyValue { get; init; }
	}

	[Test]
	public void IntLessThanRecordWhenZero()
	{
		var instance = new IntLessThanRecord { IntValue = 0 };

		var errors = IntLessThanRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void IntLessThanRecordWhenNotZero()
	{
		var instance = new IntLessThanRecord { IntValue = 1 };

		var errors = IntLessThanRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntLessThanRecord.IntValue),
					ErrorMessage = "'Int Value' must be less than or equal to '0'.",
				},
			],
			errors
		);
	}

	[Test]
	public void StringLessThanRecordWhenLessThan()
	{
		var instance = new StringLessThanRecord { StringValue = "Foo", KeyValue = "Foo" };

		var errors = StringLessThanRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void StringLessThanRecordWhenNotLessThan()
	{
		var instance = new StringLessThanRecord { StringValue = "Qaz", KeyValue = "Foo" };

		var errors = StringLessThanRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringLessThanRecord.StringValue),
					ErrorMessage = "'String Value' must be less than or equal to 'Foo'.",
				},
			],
			errors
		);
	}
}
