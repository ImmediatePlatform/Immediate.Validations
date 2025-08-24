using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class ReferenceTypeNullTests
{
	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		public required CommandObject NotNull { get; init; }
		public required CommandObject? Null { get; init; }
	}

	[Validate]
	public sealed partial record CommandObject : IValidationTarget<CommandObject>
	{
		public required string Id { get; init; }
	}

	[Fact]
	public void ValidRecordAllNotNull()
	{
		var record = new Command
		{
			NotNull = new()
			{
				Id = "Test1",
			},
			Null = new()
			{
				Id = "Test2",
			},
		};

		var errors = Command.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void ValidRecordNullIsNull()
	{
		var record = new Command
		{
			NotNull = new()
			{
				Id = "Test1",
			},
			Null = null,
		};

		var errors = Command.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void InvalidRecordNotNullIsNull()
	{
		var record = new Command
		{
			NotNull = null!,
			Null = null,
		};

		var errors = Command.Validate(record);

		Assert.Equal(
			[
				new ValidationError()
				{
					PropertyName = "NotNull",
					ErrorMessage = "'Not Null' must not be null.",
				},
			],
			errors
		);
	}
}
