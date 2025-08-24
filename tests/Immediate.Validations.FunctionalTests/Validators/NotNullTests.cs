using System.Diagnostics.CodeAnalysis;
using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class NotNullTests
{
	[Validate]
	public partial record StringRecord : IValidationTarget<StringRecord>
	{
		public required string StringValue { get; init; }
	}

	[Validate]
	public partial record AllowNullRecord : IValidationTarget<AllowNullRecord>
	{
		[AllowNull]
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
					ErrorMessage = "'String Value' must not be null.",
				},
			],
			errors
		);
	}

	[Fact]
	public void StringAllowedNullTestWhenNotNull()
	{
		var instance = new AllowNullRecord { StringValue = "Hello World!" };

		var errors = AllowNullRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void StringAllowedNullTestWhenNull()
	{
		var instance = new AllowNullRecord { StringValue = null };

		var errors = AllowNullRecord.Validate(instance);

		Assert.Empty(errors);
	}
}
