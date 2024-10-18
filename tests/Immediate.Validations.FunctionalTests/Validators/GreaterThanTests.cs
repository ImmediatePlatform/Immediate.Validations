using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class GreaterThanTests
{
	[Validate]
	public partial record IntGreaterThanRecord : IValidationTarget<IntGreaterThanRecord>
	{
		[GreaterThan(0)]
		public required int IntValue { get; init; }
	}

	[Validate]
	public partial record StringGreaterThanRecord : IValidationTarget<StringGreaterThanRecord>
	{
		[GreaterThan(nameof(KeyValue))]
		public required string StringValue { get; init; }
		public required string KeyValue { get; init; }
	}

	[Test]
	public void IntGreaterThanRecordWhenZero()
	{
		var instance = new IntGreaterThanRecord { IntValue = 1 };

		var errors = IntGreaterThanRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void IntGreaterThanRecordWhenNotZero()
	{
		var instance = new IntGreaterThanRecord { IntValue = 0 };

		var errors = IntGreaterThanRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntGreaterThanRecord.IntValue),
					ErrorMessage = "'Int Value' must be greater than '0'.",
				}
			],
			errors
		);
	}

	[Test]
	public void StringGreaterThanRecordWhenGreaterThan()
	{
		var instance = new StringGreaterThanRecord { StringValue = "Qax", KeyValue = "Foo" };

		var errors = StringGreaterThanRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void StringGreaterThanRecordWhenNotGreaterThan()
	{
		var instance = new StringGreaterThanRecord { StringValue = "Foo", KeyValue = "Foo" };

		var errors = StringGreaterThanRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringGreaterThanRecord.StringValue),
					ErrorMessage = "'String Value' must be greater than 'Foo'.",
				}
			],
			errors
		);
	}
}
