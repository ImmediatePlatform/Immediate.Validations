using System.Diagnostics.CodeAnalysis;

namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a <see cref="string"/> property to indicate that the value should have a minLength at least <paramref
///     name="minLength"/>.
/// </summary>
/// <param name="minLength">
///	    The minimum minLength of the <see cref="string"/>.
/// </param>
public sealed class MinLengthAttribute(
	[TargetType]
	object minLength
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the value should have a minLength at least <paramref name="minLength"/>.
	/// </summary>
	/// <param name="target">
	///	    The value to validate.
	/// </param>
	/// <param name="minLength">
	///	    The minimum valid minLength for the string <paramref name="target"/>.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Will already by validated by IV first.")]
	public static bool ValidateProperty(string target, int minLength) =>
		target.Length >= minLength;

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public const string DefaultMessage = "'{PropertyName}' must be more than {MinLengthValue} characters.";
}
