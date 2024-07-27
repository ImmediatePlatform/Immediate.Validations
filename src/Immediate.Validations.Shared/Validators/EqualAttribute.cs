namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should be equal to <paramref name="comparison"/>.
/// </summary>
/// <param name="comparison">
///		The value that the applied property should be equal to.
/// </param>
public sealed class EqualAttribute(
	[TargetType]
	object comparison
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the value of the applied property is equal to <paramref name="comparison"/>.
	/// </summary>
	/// <param name="target">
	///	    The value to validate.
	/// </param>
	/// <param name="comparison">
	///		The value that the applied property should be equal to.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T target, T comparison) =>
		EqualityComparer<T>.Default.Equals(target, comparison);

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage { get; } = ValidationConfiguration.Localizer[nameof(EmptyAttribute)].Value;
}
