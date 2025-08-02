using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class CollectionTests
{
	[Validate]
	public sealed partial record ListValidationTargetRecord : IValidationTarget<ListValidationTargetRecord>
	{
		public required IReadOnlyCollection<IReadOnlyCollection<ReferenceTypeNullTests.CommandObject?>> Commands { get; init; }
	}

	[Validate]
	public sealed partial record ListStringRecord : IValidationTarget<ListStringRecord>
	{
		[NotEmpty]
		public required IReadOnlyList<IReadOnlyList<string>> Strings { get; init; }
	}

	[Test]
	public void ValidRecordNoErrors()
	{
		var record = new ListStringRecord { Strings = [["Hello World!"]] };

		var errors = ListStringRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Test]
	public void InvalidRecordNullProperty()
	{
		var record = new ListStringRecord { Strings = null! };

		var errors = ListStringRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Strings",
					ErrorMessage = "'Strings' must not be null.",
				},
			],
			errors
		);
	}

	[Test]
	public void InvalidRecordEmptyProperty()
	{
		var record = new ListStringRecord { Strings = [] };

		var errors = ListStringRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Strings",
					ErrorMessage = "'Strings' must not be empty.",
				},
			],
			errors
		);
	}

	[Test]
	public void InvalidRecordNullFirstLevel()
	{
		var record = new ListStringRecord
		{
			Strings =
			[
				["Hello World!"],
				null!,
				["Test"],
				[],
			],
		};

		var errors = ListStringRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Strings[1]",
					ErrorMessage = "'Strings[1]' must not be null.",
				},
				new()
				{
					PropertyName = "Strings[3]",
					ErrorMessage = "'Strings[3]' must not be empty.",
				},
			],
			errors
		);
	}

	[Test]
	public void InvalidRecordNullSecondLevel()
	{
		var record = new ListStringRecord
		{
			Strings =
			[
				[
					"Hello World!",
					null!,
					"John Doe",
				],
				null!,
				[
					"",
					"Test",
					null!,
				],
				[],
			],
		};

		var errors = ListStringRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Strings[0][1]",
					ErrorMessage = "'Strings[0][1]' must not be null.",
				},
				new()
				{
					PropertyName = "Strings[1]",
					ErrorMessage = "'Strings[1]' must not be null.",
				},
				new()
				{
					PropertyName = "Strings[2][2]",
					ErrorMessage = "'Strings[2][2]' must not be null.",
				},
				new()
				{
					PropertyName = "Strings[3]",
					ErrorMessage = "'Strings[3]' must not be empty.",
				},
			],
			errors
		);
	}

	[Test]
	public void ValidCommandNoErrors()
	{
		var record = new ListValidationTargetRecord { Commands = [[new() { Id = "ABC-123" }]] };

		var errors = ListValidationTargetRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Test]
	public void InvalidCommandNullProperty()
	{
		var record = new ListValidationTargetRecord { Commands = null! };

		var errors = ListValidationTargetRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Commands",
					ErrorMessage = "'Commands' must not be null.",
				},
			],
			errors
		);
	}

	[Test]
	public void InvalidCommandNullFirstLevel()
	{
		var record = new ListValidationTargetRecord
		{
			Commands =
			[
				[new() { Id = "ABC-123" }],
				null!,
				[new() { Id = null! }],
				null!,
			],
		};

		var errors = ListValidationTargetRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Commands[1]",
					ErrorMessage = "'Commands[1]' must not be null.",
				},
				new()
				{
					PropertyName = "Commands[2][0].Id",
					ErrorMessage = "'Id' must not be null.",
				},
				new()
				{
					PropertyName = "Commands[3]",
					ErrorMessage = "'Commands[3]' must not be null.",
				},
			],
			errors
		);
	}

	[Test]
	public void InvalidCommandNullSecondLevel()
	{
		var record = new ListValidationTargetRecord
		{
			Commands =
			[
				[
					new() { Id = "ABC-123" },
					null,
					new() { Id = "ABC-124" },
				],
				null!,
				[
					new() { Id = null! },
					new() { Id = "ABC-125" },
					null,
				],
				null!,
			],
		};

		var errors = ListValidationTargetRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Commands[1]",
					ErrorMessage = "'Commands[1]' must not be null.",
				},
				new()
				{
					PropertyName = "Commands[2][0].Id",
					ErrorMessage = "'Id' must not be null.",
				},
				new()
				{
					PropertyName = "Commands[3]",
					ErrorMessage = "'Commands[3]' must not be null.",
				},
			],
			errors
		);
	}
}
