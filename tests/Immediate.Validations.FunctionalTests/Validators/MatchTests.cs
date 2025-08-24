using System.Text.RegularExpressions;
using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class MatchTests
{
	[Validate]
	public sealed partial record InvalidMatchAttributeRecord : IValidationTarget<InvalidMatchAttributeRecord>
	{
		[Match]
		public required string StringValue { get; init; }
	}

	[Validate]
	public sealed partial record ExprMatchRecord : IValidationTarget<ExprMatchRecord>
	{
		[Match(@"^\d+$")]
		public required string StringValue { get; init; }
	}

	[Validate]
	public sealed partial record RegexMatchRecord : IValidationTarget<RegexMatchRecord>
	{
		[GeneratedRegex(@"^\d+$")]
		private static partial Regex AllDigitsRegex();

		[Match(regex: nameof(AllDigitsRegex))]
		public required string StringValue { get; init; }
	}

	[Validate]
	public sealed partial record ExprQuoteMatchRecord : IValidationTarget<ExprQuoteMatchRecord>
	{
		[Match("\t\n^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$")]
		public required string UnusualRegexValue { get; init; }
	}

	[Fact]
	public void InvalidMatchThrows()
	{
		var record = new InvalidMatchAttributeRecord { StringValue = "", };

		var ex = Assert.Throws<ArgumentException>(() => InvalidMatchAttributeRecord.Validate(record));

		Assert.Equal("Both `regex` and `expr` are `null`. At least one must be provided.", ex.Message);
	}

	[Fact]
	public void MatchExprWhenValid()
	{
		var record = new ExprMatchRecord { StringValue = "123" };

		var errors = ExprMatchRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void MatchExprWhenInvalid()
	{
		var record = new ExprMatchRecord { StringValue = "asdf" };

		var errors = ExprMatchRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "StringValue",
					ErrorMessage = "'String Value' is not in the correct format.",
				},
			],
			errors
		);
	}

	[Fact]
	public void MatchRegexWhenValid()
	{
		var record = new RegexMatchRecord { StringValue = "123", };

		var errors = RegexMatchRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void MatchRegexWhenInvalid()
	{
		var record = new RegexMatchRecord { StringValue = "asdf", };

		var errors = RegexMatchRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "StringValue",
					ErrorMessage = "'String Value' is not in the correct format.",
				},
			],
			errors
		);
	}
}
