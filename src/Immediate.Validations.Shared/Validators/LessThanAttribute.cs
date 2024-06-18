namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should be less than <paramref name="comparison"/>.
/// </summary>
/// <param name="comparison">
///		The value that the applied property should be less than.
/// </param>
public sealed class LessThanAttribute(
	[TargetType]
	object comparison
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the value of the applied property is less than <paramref name="comparison"/>.
	/// </summary>
	/// <param name="target">
	///	    The value to validate.
	/// </param>
	/// <param name="comparison">
	///		The value that the applied property should be less than.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T target, T comparison) =>
		Comparer<T>.Default.Compare(target, comparison) < 0;

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public const string DefaultMessage = "'{PropertyName}' must be less than '{ComparisonValue}'.";
}
