using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Immediate.Validations.Shared;
using Vogen;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[ValueObject(deserializationStrictness: DeserializationStrictness.AllowAnything)]
[SuppressMessage(
	"Design",
	"CA1036:Override methods on comparable types",
	Justification = "Intentionally not implemented in Vogen"
)]
public readonly partial struct UserId
{
	public static Validation Validate(int value) =>
		value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
}

public sealed partial class VogenTests
{
	[Validate]
	public sealed partial record VogenRecord : IValidationTarget<VogenRecord>
	{
		public required UserId UserId { get; init; }
	}

	[Fact]
	public void ValidUserIdWithNoErrors()
	{
		var record = JsonSerializer.Deserialize<VogenRecord>(
			/*lang=json,strict*/
			"""
			{
				"UserId": 1
			}
			"""
		);

		var errors = VogenRecord.Validate(record);

		Assert.Empty(errors);
	}

	[Fact]
	public void InvalidUserIdWithErrors()
	{
		var record = JsonSerializer.Deserialize<VogenRecord>(
			/*lang=json,strict*/
			"""
			{
				"UserId": -1
			}
			"""
		);

		var errors = VogenRecord.Validate(record);

		Assert.Equal(
			[
				new()
				{
					PropertyName = "UserId",
					ErrorMessage = "Must be greater than zero.",
				}
			],
			errors
		);
	}
}
