using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class CollectionTests
{
	[Validate]
	public sealed partial record ListValidationTargetRecord : IValidationTarget<ListValidationTargetRecord>
	{
		public required IReadOnlyList<IReadOnlyList<ReferenceTypeNullTests.CommandObject?>> Commands { get; init; }
	}

	[Validate]
	public sealed partial record ListStringRecord : IValidationTarget<ListStringRecord>
	{
		[NotEmptyOrWhiteSpace]
		public required IReadOnlyList<IReadOnlyList<string>> Strings { get; init; }
	}

	[Fact]
	public void ValidRecordNoErrors()
	{
		var record = new ListStringRecord { Strings = [["Hello World!"]] };

		var errors = ListStringRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void InvalidRecordNullProperty()
	{
		var record = new ListStringRecord { Strings = null! };

		var errors = ListStringRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Strings",
					ErrorMessage = "Property must not be `null`.",
				}
			],
			errors
		);
	}

	[Fact]
	public void InvalidRecordNullFirstLevel()
	{
		var record = new ListStringRecord
		{
			Strings =
			[
				["Hello World!"],
				null!,
				["Test"],
				null!,
			],
		};

		var errors = ListStringRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Strings[1]",
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Strings[3]",
					ErrorMessage = "Property must not be `null`.",
				},
			],
			errors
		);
	}

	[Fact]
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
				null!,
			],
		};

		var errors = ListStringRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Strings[0][1]",
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Strings[1]",
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Strings[2][0]",
					ErrorMessage = "Property must not be `null` or whitespace.",
				},
				new()
				{
					PropertyName = "Strings[2][2]",
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Strings[3]",
					ErrorMessage = "Property must not be `null`.",
				},
			],
			errors
		);
	}

	[Fact]
	public void ValidCommandNoErrors()
	{
		var record = new ListValidationTargetRecord { Commands = [[new() { Id = "ABC-123" }]] };

		var errors = ListValidationTargetRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void InvalidCommandNullProperty()
	{
		var record = new ListValidationTargetRecord { Commands = null! };

		var errors = ListValidationTargetRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "Commands",
					ErrorMessage = "Property must not be `null`.",
				}
			],
			errors
		);
	}

	[Fact]
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
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Commands[2][0].Id",
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Commands[3]",
					ErrorMessage = "Property must not be `null`.",
				},
			],
			errors
		);
	}

	[Fact]
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
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Commands[2][0].Id",
					ErrorMessage = "Property must not be `null`.",
				},
				new()
				{
					PropertyName = "Commands[3]",
					ErrorMessage = "Property must not be `null`.",
				},
			],
			errors
		);
	}
}
