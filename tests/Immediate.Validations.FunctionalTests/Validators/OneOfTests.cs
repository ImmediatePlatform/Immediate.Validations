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

	[Validate]
	public partial record IntRecord : IValidationTarget<IntRecord>
	{
		[OneOf(123, 456, 789)]
		public required int IntValue { get; init; }
	}

	[Validate]
	public partial record IntFieldRecord : IValidationTarget<IntFieldRecord>
	{
		[OneOf(nameof(s_validValues))]
		public required int IntValue { get; init; }

		private static readonly int[] s_validValues = [123, 456, 789];
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
					ErrorMessage = "'String Value' was not one of the specified values: 123, 456, 789.",
				}
			],
			errors
		);
	}

	[Fact]
	public void IntValueIsOneOfNoErrors()
	{
		var instance = new IntRecord { IntValue = 123 };

		var errors = IntRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void IntValueIsNotOneOfHasErrors()
	{
		var instance = new IntRecord { IntValue = 124 };

		var errors = IntRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntRecord.IntValue),
					ErrorMessage = "'Int Value' was not one of the specified values: 123, 456, 789.",
				}
			],
			errors
		);
	}

	[Fact]
	public void IntFieldValueIsOneOfNoErrors()
	{
		var instance = new IntFieldRecord { IntValue = 123 };

		var errors = IntFieldRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void IntFieldValueIsNotOneOfHasErrors()
	{
		var instance = new IntFieldRecord { IntValue = 124 };

		var errors = IntFieldRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntFieldRecord.IntValue),
					ErrorMessage = "'Int Value' was not one of the specified values: 123, 456, 789.",
				}
			],
			errors
		);
	}
}
