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
///		Extension method for List&lt;ValidationError&gt;.
/// </summary>
public static class ValidationErrorExtensions
{
	/// <summary>
	///		Conditionally adds a validation message if the validation failed.
	/// </summary>
	/// <param name="list">
	///		A list of validation messages, to which to add the message.
	/// </param>
	/// <param name="validation">
	///		The results of a <see cref="ValidatorAttribute"/> validation.
	/// </param>
	/// <param name="propertyName">
	///		The name of the property that was validated.
	/// </param>
	/// <param name="overrideMessage">
	///		A specific message to override the message provided by the validator.
	/// </param>
	public static void Add(
		this List<ValidationError> list,
		(bool Invalid, string? DefaultMessage) validation,
		string propertyName,
		string? overrideMessage = null
	)
	{
		ArgumentNullException.ThrowIfNull(list);
		ArgumentNullException.ThrowIfNull(propertyName);

		if (validation.Invalid)
		{
			var message = string.IsNullOrWhiteSpace(overrideMessage)
				? validation.DefaultMessage!
				: overrideMessage;

			list.Add(new()
			{
				PropertyName = propertyName,
				ErrorMessage = message,
			});
		}
	}
}
