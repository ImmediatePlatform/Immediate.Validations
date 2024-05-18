namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should not be equal to <paramref name="operand"/>.
/// </summary>
/// <param name="operand">
///		The value that the applied property should not be.
/// </param>
public sealed class NotEqualAttribute(
	[TargetType]
	object operand
) : ValidatorAttribute
{
	/// <summary>
	///		The value that the applied property should not be.
	/// </summary>
	public object Operand { get; } = operand;

	/// <summary>
	///	    Validates that the value of the applied property is not equal to <paramref name="operand"/>.
	/// </summary>
	/// <param name="target">
	///	    The value to validate.
	/// </param>
	/// <param name="operand">
	///		The value that the applied property should not be.
	/// </param>
	/// <returns>
	///	    A <see cref="ValueTuple{T1, T2}"/> indicating whether the property is valid or not, along with an error
	///     message if the property is not valid.
	/// </returns>
	public static (bool Invalid, string? Message) ValidateProperty<T>(T target, T operand) =>
		!EqualityComparer<T>.Default.Equals(target, operand)
			? default
			: (true, $"Value '{target}' is equal to '{operand}'");
}
