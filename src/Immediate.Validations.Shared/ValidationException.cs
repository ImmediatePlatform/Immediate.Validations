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
[SuppressMessage(
	"Design",
	"CA1002:Do not expose generic lists",
	Justification = "Validation system uses concrete List<> types for performance."
)]
public sealed class ValidationException : Exception
{
	private ValidationException(List<ValidationError> errors)
		: base(BuildErrorMessage("Validation failed: ", errors))
	{
		Errors = errors;
	}

	internal ValidationException(string message, List<ValidationError> errors)
		: base(BuildErrorMessage(message, errors))
	{
		Errors = errors;
	}

	private static string? BuildErrorMessage(string message, List<ValidationError> errors)
	{
		var messages = errors
			.Select(e => $"{Environment.NewLine} -- {e.PropertyName}: {e.ErrorMessage}")
			.Prepend(message);
		return string.Concat(messages);
	}

	/// <summary>
	///		The list of validation failures.
	/// </summary>
	public IReadOnlyList<ValidationError> Errors { get; }

	/// <summary>
	///	    Validates an object and throws a <see cref="ValidationException"/> is there are any validation errors.
	/// </summary>
	/// <typeparam name="T">
	///		The type of the object to be validated.
	/// </typeparam>
	/// <param name="obj">
	///		The object to be validated.
	/// </param>
	/// <exception cref="ValidationException">
	///		Thrown if there are any errors.
	/// </exception>
	public static void ThrowIfInvalid<T>(T obj)
		where T : IValidationTarget<T>
	{
		ThrowIfInvalid(T.Validate(obj));
	}

	/// <summary>
	///	    Validates an object and throws a <see cref="ValidationException"/> is there are any validation errors.
	/// </summary>
	/// <typeparam name="T">
	///		The type of the object to be validated.
	/// </typeparam>
	/// <param name="obj">
	///		The object to be validated.
	/// </param>
	/// <param name="message">
	///		The base message for the exception.
	/// </param>
	/// <exception cref="ValidationException">
	///		Thrown if there are any errors.
	/// </exception>
	public static void ThrowIfInvalid<T>(T obj, string message)
		where T : IValidationTarget<T>
	{
		ThrowIfInvalid(T.Validate(obj), message);
	}

	/// <summary>
	///	    Validates an object and throws a <see cref="ValidationException"/> is there are any validation errors.
	/// </summary>
	/// <typeparam name="T">
	///		The type of the object to be validated.
	/// </typeparam>
	/// <param name="obj">
	///		The object to be validated.
	/// </param>
	/// <param name="messageFunc">
	///		A message generator, based on the <paramref name="obj"/>, for the base message of the exception.
	/// </param>
	/// <exception cref="ValidationException">
	///		Thrown if there are any errors.
	/// </exception>
	public static void ThrowIfInvalid<T>(T obj, Func<T, string> messageFunc)
		where T : IValidationTarget<T>
	{
		ArgumentNullException.ThrowIfNull(messageFunc);

		var errors = T.Validate(obj);
		if (errors is { Count: > 0 })
			ThrowValidationException(errors, messageFunc(obj));
	}

	/// <summary>
	///	    Throws a <see cref="ValidationException"/> if there are any errors.
	/// </summary>
	/// <param name="errors">
	///	    A list of errors generated when validating an object.
	/// </param>
	/// <exception cref="ValidationException">
	///		Thrown if there are any errors.
	/// </exception>
	public static void ThrowIfInvalid(List<ValidationError> errors)
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
	public static void ThrowIfInvalid(List<ValidationError> errors, string message)
	{
		if (errors is { Count: > 0 })
			ThrowValidationException(errors, message);
	}

	private static void ThrowValidationException(List<ValidationError> errors) =>
		throw new ValidationException(errors);

	private static void ThrowValidationException(List<ValidationError> errors, string message) =>
		throw new ValidationException(message, errors);
}
