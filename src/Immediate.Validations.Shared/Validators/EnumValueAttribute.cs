namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value of the property should be a defined value of the <see
///     langword="enum" />.
/// </summary>
public sealed class EnumValueAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <see langword="enum" /> <paramref name="value"/> is properly defined.
	/// </summary>
	/// <typeparam name="T">
	///	    The type of the provided <see langword="enum" /> value.
	/// </typeparam>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T value)
		where T : struct, Enum => Enum.IsDefined(value);

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage { get; } = ValidationConfiguration.Localizer[nameof(EmptyAttribute)].Value;
}
