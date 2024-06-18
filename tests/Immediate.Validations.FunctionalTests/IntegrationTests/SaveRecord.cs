using System.ComponentModel;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

[Handler]
[Behaviors(typeof(ValidationBehavior<,>))]
public static partial class SaveRecord
{
	public enum Status
	{
		None = 0,
		Open,
		Pending,
		Deleted,
		Closed,
	}

	public sealed class GreaterThanAttribute : ValidatorAttribute
	{
		public required int Operand { get; init; }

		public static bool ValidateProperty(int value, int operand) =>
			value > operand;

		public const string DefaultMessage = "Value `{PropertyValue}` is not greater than `{OperandValue}`.";
	}

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[NotEmpty(Message = "Name must be provided.")]
		[MaxLength(MaxLength)]
		public required string Name { get; init; }

		[Description("test")]
		public required Status Status { get; init; }

		[GreaterThan(Operand = 0)]
		public required int Value { get; init; }

		public const int MaxLength = 250;
	}

	private static ValueTask HandleAsync(
		Command _,
		CancellationToken __
	)
	{
		return ValueTask.CompletedTask;
	}
}
