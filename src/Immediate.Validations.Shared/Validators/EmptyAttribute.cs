using System.Collections;

namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should be empty.
/// </summary>
public sealed class EmptyAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <paramref name="value"/> is empty.
	/// </summary>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid (aka empty); <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty<T>(T value) =>
		IsEmpty(value);

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public const string DefaultMessage = "'{PropertyName}' must be empty.";

	internal static bool IsEmpty<T>(T value) =>
		value switch
		{
			null => true,
			string s when string.IsNullOrWhiteSpace(s) => true,
			ICollection { Count: 0 } => true,
			_ => EqualityComparer<T>.Default.Equals(value, default)
		};
}
