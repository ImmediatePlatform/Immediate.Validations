namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should be equal to the value <see cref="Operand"/>.
/// </summary>
public sealed class EqualAttribute(
	[TargetType]
	object operand
) : ValidatorAttribute
{
	/// <summary>
	///		The value that the applied property should be.
	/// </summary>
	public object Operand { get; } = operand;

	/// <summary>
	///		Validates that the 
	/// </summary>
	/// <param name="target"></param>
	/// <param name="operand"></param>
	/// <returns></returns>
	public static (bool Invalid, string? Message) ValidateProperty<T>(T target, T operand) =>
		EqualityComparer<T>.Default.Equals(target, operand)
			? default
			: (true, $"Value '{target}' is not equal to '{operand}'");
}
