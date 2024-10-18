using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class StructTests
{
	[Validate]
	public readonly partial record struct RecordStructTarget : IValidationTarget<RecordStructTarget>
	{
		public required string StringProperty { get; init; }
	}

	[Validate]
	public readonly partial record struct StructTarget : IValidationTarget<StructTarget>
	{
		public required string StringProperty { get; init; }
	}

	[Test]
	public void ValidRecordStruct()
	{
		var rs = new RecordStructTarget { StringProperty = "Hello World!" };

		var errors = RecordStructTarget.Validate(rs);

		Assert.Empty(errors);
	}

	[Test]
	public void InvalidRecordStructNullProperty()
	{
		var rs = new RecordStructTarget { StringProperty = null! };

		var errors = RecordStructTarget.Validate(rs);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "StringProperty",
					ErrorMessage = "'String Property' must not be null.",
				}
			],
			errors
		);
	}

	[Test]
	public void ValidStruct()
	{
		var rs = new StructTarget { StringProperty = "Hello World!" };

		var errors = StructTarget.Validate(rs);

		Assert.Empty(errors);
	}

	[Test]
	public void InvalidStructNullProperty()
	{
		var rs = new StructTarget { StringProperty = null! };

		var errors = StructTarget.Validate(rs);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "StringProperty",
					ErrorMessage = "'String Property' must not be null.",
				}
			],
			errors
		);
	}
}
