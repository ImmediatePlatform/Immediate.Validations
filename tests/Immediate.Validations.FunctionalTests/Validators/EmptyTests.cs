using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class EmptyTests
{
	[Validate]
	public partial record StringRecord : IValidationTarget<StringRecord>
	{
		[Empty]
		public required string StringValue { get; init; }
	}

	[Validate]
	public partial record IntRecord : IValidationTarget<IntRecord>
	{
		[Empty]
		public required int IntValue { get; init; }
	}

	[Validate]
	public partial record CollectionRecord : IValidationTarget<CollectionRecord>
	{
		[Empty]
		public required ICollection<int> CollectionValue { get; init; }
	}

	[Fact]
	public void StringEmptyTestWhenEmpty()
	{
		var instance = new StringRecord { StringValue = "  " };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void StringEmptyTestWhenNotEmpty()
	{
		var instance = new StringRecord { StringValue = "Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "Property must be empty.",
				}
			],
			errors
		);
	}

	[Fact]
	public void IntEmptyTestWhenEmpty()
	{
		var instance = new IntRecord { IntValue = 0 };

		var errors = IntRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void IntEmptyTestWhenNotEmpty()
	{
		var instance = new IntRecord { IntValue = 5 };

		var errors = IntRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntRecord.IntValue),
					ErrorMessage = "Property must be empty.",
				}
			],
			errors
		);
	}

	[Fact]
	public void CollectionEmptyTestWhenEmpty()
	{
		var instance = new CollectionRecord { CollectionValue = [] };

		var errors = CollectionRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void CollectionEmptyTestWhenNotEmpty()
	{
		var instance = new CollectionRecord { CollectionValue = [1, 2, 3, 4] };

		var errors = CollectionRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(CollectionRecord.CollectionValue),
					ErrorMessage = "Property must be empty.",
				}
			],
			errors
		);
	}
}
