namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the property should not be <see langword="null"/>.
/// </summary>
public sealed class NotNullAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <paramref name="value"/> is not <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">
	///	    The type of the provided value.
	/// </typeparam>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T value) =>
		value is not null;

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage => ValidationConfiguration.Localizer[nameof(NotNullAttribute)].Value;
}
