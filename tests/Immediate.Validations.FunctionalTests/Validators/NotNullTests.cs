using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class NotNullTests
{
	[Validate]
	public partial record StringRecord
	{
		public required string StringValue { get; init; }
	}

	[Fact]
	public void StringNotNullTestWhenNotNull()
	{
		var instance = new StringRecord { StringValue = "Hello World!" };

		var errors = StringRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void StringNotNullTestWhenNull()
	{
		var instance = new StringRecord { StringValue = null! };

		var errors = StringRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(StringRecord.StringValue),
					ErrorMessage = "Property must not be `null`." ,
				}
			],
			errors
		);
	}
}
