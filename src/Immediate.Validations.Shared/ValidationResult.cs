using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Immediate.Validations.Shared;

/// <summary>
///		Represents the result of an in-progress or completed validation.
/// </summary>
public sealed partial class ValidationResult : IEnumerable<ValidationError>
{
	[GeneratedRegex("{([^{}:]+)(?::([^{}]+))?}")]
	private static partial Regex FormatRegex();

	private List<ValidationError>? _errors;

	/// <summary>
	///		Indicates whether the validation was successful.
	/// </summary>
	public bool IsValid => _errors is null or [];

	/// <summary>
	///		The list of errors held by the current validation attempt.
	/// </summary>
	public IReadOnlyList<ValidationError> Errors => _errors ?? [];

	IEnumerator<ValidationError> IEnumerable<ValidationError>.GetEnumerator() => (_errors ?? []).AsEnumerable().GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<ValidationError>).GetEnumerator();

	/// <summary>
	///	    Unconditionally add a <see cref="ValidationError"/>.
	/// </summary>
	/// <param name="validationError">
	///		The <see cref="ValidationError"/> to add to the current list.
	/// </param>
	public void Add(
		ValidationError validationError
	)
	{
		(_errors ??= []).Add(validationError);
	}

	/// <summary>
	///	    Unconditionally add a <see cref="ValidationError"/> for <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="propertyName">
	///	    The name of the property that failed the validation.
	/// </param>
	/// <param name="messageTemplate">
	///	    A template message, which will be filled in with the values from <paramref name="arguments"/>.
	/// </param>
	/// <param name="arguments">
	///	    The values which can be used with <paramref name="messageTemplate"/> to build the final validation message.
	/// </param>
	public void Add(
		string propertyName,
		string messageTemplate,
		Dictionary<string, object?>? arguments = default
	)
	{
		(_errors ??= []).Add(new()
		{
			PropertyName = propertyName,
			ErrorMessage =
				arguments is null
				? messageTemplate
				: FormatRegex().Replace(
					messageTemplate,
					m =>
					{
						var key = m.Groups[1].Value;

						if (!arguments.TryGetValue(key, out var value))
							return m.Value;

						if (m.Groups[2].Success)
							return string.Format(provider: null, $"{{0:{m.Groups[2].Value}}}", value);

						return value?.ToString()!;
					}
				),
		});
	}

	/// <summary>
	///	    Evaluates an <paramref name="expression"/> and conditionally adds a <see cref="ValidationError"/> if the
	///     property is invalid.
	/// </summary>
	/// <param name="expression">
	///	    An expression representing a property as well as how it should be validated.
	/// </param>
	/// <param name="overrideMessage">
	///	    An optional message template to be used instead of the default message for the <see
	///     cref="ValidatorAttribute"/> that was used.
	/// </param>
	/// <exception cref="NotSupportedException">
	///	    If an invalid <see cref="Expression"/> is provided, for example, 
	/// </exception>
	/// <remarks>
	///	    The <see cref="Expression" /> must follow the form
	///	    <code>() => ValidatorAttribute.ValidateProperty(t.Property/*, other parameters as necessary */)</code>
	///	    where <c>ValidatorAttribute</c> is the validator and <c>t.Property</c> is the value that should be 
	///	    validated. 
	/// </remarks>
	public void Add(
		Expression<Func<bool>> expression,
		string? overrideMessage = null
	)
	{
		throw new NotImplementedException();
	}
}
