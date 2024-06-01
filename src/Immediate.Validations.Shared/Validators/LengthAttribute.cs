using System.Diagnostics.CodeAnalysis;

namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a <see cref="string"/> property to indicate that the value should have a length of exactly <paramref
///     name="length"/>.
/// </summary>
/// <param name="length">
///	    The maximum length of the <see cref="string"/>.
/// </param>
public sealed class LengthAttribute(
	[TargetType]
	object length
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the <see cref="string"/> has a length of exactly <paramref name="length"/>.
	/// </summary>
	/// <param name="target">
	///	    The value to validate.
	/// </param>
	/// <param name="length">
	///		The valid length for the string <paramref name="target"/>.
	/// </param>
	/// <returns>
	///	    A <see cref="ValueTuple{T1, T2}"/> indicating whether the property is valid or not, along with an error
	///     message if the property is not valid.
	/// </returns>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Will already by validated by IV first.")]
	public static (bool Invalid, string? Message) ValidateProperty(string target, int length) =>
		target.Length == length
			? default
			: (true, $"String is of length '{target.Length}', which is {(target.Length < length ? "shorter" : "longer")} than the allowed length of '{length}'.");
}
