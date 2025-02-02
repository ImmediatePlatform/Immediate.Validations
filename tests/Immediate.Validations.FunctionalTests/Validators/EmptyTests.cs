using Immediate.Validations.Shared;

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

	[Test]
	public void StringEmptyTestWhenEmpty()
	{
		var instance = new StringRecord { StringValue = "  " };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void StringEmptyTestWhenNotEmpty()
	{
		var instance = new StringRecord { StringValue = "Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "'String Value' must be empty.",
				},
			],
			errors
		);
	}

	[Test]
	public void IntEmptyTestWhenEmpty()
	{
		var instance = new IntRecord { IntValue = 0 };

		var errors = IntRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void IntEmptyTestWhenNotEmpty()
	{
		var instance = new IntRecord { IntValue = 5 };

		var errors = IntRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(IntRecord.IntValue),
					ErrorMessage = "'Int Value' must be empty.",
				},
			],
			errors
		);
	}

	[Test]
	public void CollectionEmptyTestWhenEmpty()
	{
		var instance = new CollectionRecord { CollectionValue = [] };

		var errors = CollectionRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void CollectionEmptyTestWhenNotEmpty()
	{
		var instance = new CollectionRecord { CollectionValue = [1, 2, 3, 4] };

		var errors = CollectionRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(CollectionRecord.CollectionValue),
					ErrorMessage = "'Collection Value' must be empty.",
				},
			],
			errors
		);
	}
}
