using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class CollectionTests
{
	[Validate]
	public sealed partial record ListStringRecord
	{
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
	public void ValidRecordNullProperty()
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
	public void ValidRecordNullFirstLevel()
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
	public void ValidRecordNullSecondLevel()
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
					null!,
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
					ErrorMessage = "Property must not be `null`.",
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
}
