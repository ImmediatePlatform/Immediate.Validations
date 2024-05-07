using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class NotEmptyOrWhiteSpaceTests
{
	[Validate]
	public partial record StringRecord
	{
		[NotEmptyOrWhiteSpace]
		public required string StringValue { get; init; }
	}

	[Fact]
	public void StringNotEmptyOrWhiteSpaceTestWhenNotNull()
	{
		var instance = new StringRecord { StringValue = "Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void StringNotEmptyOrWhiteSpaceTestWhenEmpty()
	{
		var instance = new StringRecord { StringValue = "" };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "Property must not be `null` or whitespace.",
				}
			],
			errors
		);
	}
}
