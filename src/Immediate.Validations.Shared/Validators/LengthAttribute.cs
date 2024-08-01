using System.Diagnostics.CodeAnalysis;

namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a <see cref="string"/> property to indicate that the value should have a length between <paramref
///     name="minLength"/> and <paramref name="maxLength"/>.
/// </summary>
/// <param name="minLength">
///	    The minimum length of the <see cref="string"/>.
/// </param>
/// <param name="maxLength">
///	    The maximum length of the <see cref="string"/>.
/// </param>
public sealed class LengthAttribute(
	[TargetType]
	object minLength,
	[TargetType]
	object maxLength
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the <see cref="string"/> has a length between <paramref name="minLength"/> and <paramref
	///     name="maxLength"/>.
	/// </summary>
	/// <param name="target">
	///	    The value to validate.
	/// </param>
	/// <param name="minLength">
	///	    The minimum length of the <see cref="string"/>.
	/// </param>
	/// <param name="maxLength">
	///	    The maximum length of the <see cref="string"/>.
	/// </param>
	/// <returns>
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Will already by validated by IV first.")]
	public static bool ValidateProperty(string target, int minLength, int maxLength) =>
		target.Length >= minLength && target.Length <= maxLength;

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage => ValidationConfiguration.Localizer[nameof(EmptyAttribute)].Value;
}
