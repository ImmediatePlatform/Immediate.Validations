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
	///	    <see langword="true" /> if the property is valid; <see langword="false" /> otherwise.
	/// </returns>
	public static bool ValidateProperty(string target, Regex? regex = null, [StringSyntax("Regex")] string? expr = null)
	{
		if (regex is null)
		{
			if (expr is null)
				ThrowInvalidArgumentsException();

			regex = new Regex(expr, RegexOptions.None, TimeSpan.FromSeconds(1));
		}

		return regex.IsMatch(target);
	}

	[DoesNotReturn]
	[SuppressMessage("Usage", "MA0015:Specify the parameter name in ArgumentException", Justification = "Argument Exception applies to multiple properties")]
	private static void ThrowInvalidArgumentsException() =>
		throw new ArgumentException("Both `regex` and `expr` are `null`. At least one must be provided.");

	/// <summary>
	///		The default message template when the property is invalid.
	/// </summary>
	public static string DefaultMessage => ValidationConfiguration.Localizer[nameof(MatchAttribute)].Value;
}
