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
	private ValidationException(IReadOnlyList<ValidationError> errors)
		: base(BuildErrorMessage("Validation failed", errors))
	{
		Title = "Validation failed";
		Errors = errors;
	}

	internal ValidationException(string message, IReadOnlyList<ValidationError> errors)
		: base(BuildErrorMessage(message, errors))
	{
		Title = message;
		Errors = errors;
	}

	private static string? BuildErrorMessage(string message, IReadOnlyList<ValidationError> errors)
	{
		var messages = errors
			.Select(e => $"{Environment.NewLine} -- {e.PropertyName}: {e.ErrorMessage}")
			.Prepend(message + ":");
		return string.Concat(messages);
	}

	/// <summary>
	///		The base error message provided to the <see cref="ValidationException"/>.
	/// </summary>
	public string Title { get; }

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
		if (!errors.IsValid)
			ThrowValidationException(errors.Errors, messageFunc(obj));
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
	public static void ThrowIfInvalid(ValidationResult errors)
	{
		ArgumentNullException.ThrowIfNull(errors);
		if (!errors.IsValid)
			ThrowValidationException(errors.Errors);
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
	public static void ThrowIfInvalid(ValidationResult errors, string message)
	{
		ArgumentNullException.ThrowIfNull(errors);
		if (!errors.IsValid)
			ThrowValidationException(errors.Errors, message);
	}

	private static void ThrowValidationException(IReadOnlyList<ValidationError> errors) =>
		throw new ValidationException(errors);

	private static void ThrowValidationException(IReadOnlyList<ValidationError> errors, string message) =>
		throw new ValidationException(message, errors);
}
