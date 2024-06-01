using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Immediate.Validations.Shared;

/// <summary>
///		Applied to a <see cref="string"/> to validate that it matches a particular <see cref="Regex"/> pattern.
/// </summary>
/// <param name="expr">
///		A <see cref="string"/> containing a <see cref="Regex"/> pattern to match the property value.
/// </param>
/// <param name="regex">
///		A <see cref="Regex"/> instance for matching property value.
/// </param>
public sealed class MatchAttribute(
	[TargetType]
	[StringSyntax("Regex")]
	object? expr = null,
	[TargetType]
	object? regex = null
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the <see cref="string"/> matches a particular <see cref="Regex"/> pattern.
	/// </summary>
	/// <param name="target">
	///		The value to validate.
	/// </param>
	/// <param name="expr">
	///		A <see cref="string"/> containing a <see cref="Regex"/> pattern to match the property value.
	/// </param>
	/// <param name="regex">
	///		A <see cref="Regex"/> instance for matching property value.
	/// </param>
	/// <returns>
	///	    A <see cref="ValueTuple{T1, T2}"/> indicating whether the property is valid or not, along with an error
	///     message if the property is not valid.
	/// </returns>
	public static (bool Invalid, string? DefaultMessage) ValidateProperty(string target, Regex? regex = null, string? expr = null)
	{
		if (regex is null)
		{
			if (expr is null)
				ThrowInvalidArgumentsException();

			regex = new Regex(expr);
		}

		return regex.IsMatch(target)
			? default
			: (true, $"Property did not satisfy regex pattern '{regex}'.");
	}

	[DoesNotReturn]
	private static void ThrowInvalidArgumentsException() =>
		throw new ArgumentException("Both `regex` and `expr` are `null`. At least one must be provided.");
}
