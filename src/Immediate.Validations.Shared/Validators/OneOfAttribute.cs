namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should one of the provided <paramref name="values"/>.
/// </summary>
/// <param name="values">
///		An array of values which the applied property should be.
/// </param>
public sealed class OneOfAttribute(
	[TargetType]
	params object[] values
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the property value is one of the specified <paramref name="values"/>.
	/// </summary>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <param name="values"></param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T value, params T[] values) =>
		values.Contains(value);

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage { get; } = ValidationConfiguration.Localizer[nameof(EmptyAttribute)].Value;
}
