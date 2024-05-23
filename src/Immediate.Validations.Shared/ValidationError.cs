namespace Immediate.Validations.Shared;

/// <summary>
///		Represents a validation failure.
/// </summary>
public sealed record ValidationError
{
	/// <summary>
	///		The name of the property which failed validation.
	/// </summary>
	public required string PropertyName { get; init; }

	/// <summary>
	///		The error message for the validation failure.
	/// </summary>
	public required string ErrorMessage { get; init; }
}
