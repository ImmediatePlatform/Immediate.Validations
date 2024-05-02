namespace Immediate.Validations.Shared.Attributes;

/// <summary>
///	    Applied to a <see langword="string"/> property to indicate that the value should not be <see langword="null"/>
///     or whitespace.
/// </summary>
public sealed class NotEmptyOrWhiteSpaceAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <see langword="string"/> <paramref name="value"/> is properly defined.
	/// </summary>
	/// <param name="value">
	///	    The <see langword="string"/> to validate.
	/// </param>
	/// <returns>
	///	    A <see cref="ValueTuple{T1, T2}"/> indicating whether the property is valid or not, along with an error
	///     message if the property is not valid.
	/// </returns>
	public static (bool Invalid, string? DefaultMessage) ValidateProperty(string value) =>
		string.IsNullOrWhiteSpace(value)
			? (true, "Property must not be `null` or whitespace.")
			: default;
}
