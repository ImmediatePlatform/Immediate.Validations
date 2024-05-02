using System.Diagnostics.CodeAnalysis;

namespace Immediate.Validations.Shared;

/// <summary>
///		An exception that represents a failed validation.
/// </summary>
[SuppressMessage(
	"Design",
	"CA1032:Implement standard exception constructors",
	Justification = "Exception is not intended for general use."
)]
public sealed class ValidationException : Exception
{
	internal ValidationException(List<ValidationError> errors)
		: base(BuildErrorMessage(errors))
	{
		Errors = errors;
	}

	private static string? BuildErrorMessage(List<ValidationError> errors)
	{
		var messages = errors
			.Select(e => $"{Environment.NewLine} -- {e.PropertyName}: {e.ErrorMessage}")
			.Prepend($"Validation failed: ");
		return string.Concat(messages);
	}

	/// <summary>
	///		The list of validation failures.
	/// </summary>
	public IReadOnlyList<ValidationError> Errors { get; }
}
