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

/// <summary>
///	    Extension methods to facilitate the throwing of <see cref="ValidationException"/> when there is an error.
/// </summary>
public static class ValidationErrorExtensions
{
	/// <summary>
	///	    Throws a <see cref="ValidationException"/> if there are any errors.
	/// </summary>
	/// <param name="errors">
	///	    A list of errors generated when validating an object.
	/// </param>
	/// <exception cref="ValidationException">
	///		Thrown if there are any errors.
	/// </exception>
	public static void ValidateAndThrow(this List<ValidationError> errors)
	{
		if (errors is { Count: > 0 })
			ThrowValidationException(errors);
	}

	/// <summary>
	///	    Throws a <see cref="ValidationException"/> if there are any errors.
	/// </summary>
	/// <param name="errors">
	///	    A list of errors generated when validating an object.
	/// </param>
	/// <param name="message">
	///		The base message for the exception.
	/// </param>
	/// <exception cref="ValidationException">
	///		Thrown if there are any errors.
	/// </exception>
	public static void ValidateAndThrow(this List<ValidationError> errors, string message)
	{
		if (errors is { Count: > 0 })
			ThrowValidationException(errors, message);
	}

	private static void ThrowValidationException(this List<ValidationError> errors) =>
		throw new ValidationException(errors);

	private static void ThrowValidationException(List<ValidationError> errors, string message) =>
		throw new ValidationException(message, errors);
}
