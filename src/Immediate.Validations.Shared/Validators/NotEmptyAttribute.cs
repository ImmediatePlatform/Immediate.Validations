namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that it should not be empty.
/// </summary>
public sealed class NotEmptyAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <paramref name="value"/> is not empty.
	/// </summary>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid (aka not-empty); <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T value) =>
		!EmptyAttribute.IsEmpty(value);

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage { get; } = ValidationConfiguration.Localizer[nameof(EmptyAttribute)].Value;
}
