using System.Diagnostics.CodeAnalysis;

namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a <see cref="string"/> property to indicate that the value should have a maxLength at most <paramref
///     name="maxLength"/>.
/// </summary>
/// <param name="maxLength">
///		The maximum maxLength of the <see cref="string"/>.
/// </param>
public sealed class MaxLengthAttribute(
	[TargetType]
	object maxLength
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the value should have a maxLength at most <paramref name="maxLength"/>.
	/// </summary>
	/// <param name="target">
	///	    The value to validate.
	/// </param>
	/// <param name="maxLength">
	///	    The maximum valid maxLength for the string <paramref name="target"/>.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Will already by validated by IV first.")]
	public static bool ValidateProperty(string target, int maxLength) =>
		target.Length <= maxLength;

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public const string DefaultMessage = "'{PropertyName}' must be less than {MaxLengthValue} characters.";
}
