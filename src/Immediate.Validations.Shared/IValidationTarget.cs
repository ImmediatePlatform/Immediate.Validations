using System.Diagnostics.CodeAnalysis;

namespace Immediate.Validations.Shared;

/// <summary>
///	    This represents a class that has a <see cref="Validate(T)"/> method to validate instances of that class.
/// </summary>
/// <typeparam name="T">
///		The type which should be validated.
/// </typeparam>
public interface IValidationTarget<T>
{
	/// <summary>
	///	    A method which can be used to validate instances of the type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="target">
	///	    An instance of type <typeparamref name="T"/> which should be validated.
	/// </param>
	/// <returns>
	///	    A <see cref="ValidationResult" /> containing the result of the validation on <paramref name="target"/>.
	/// </returns>
	[SuppressMessage(
		"Design",
		"CA1000:Do not declare static members on generic types",
		Justification = "This is a static method to validate an instance of the self type."
	)]
	static abstract ValidationResult Validate(T? target);

	/// <summary>
	///	    A method which can be used to validate instances of the type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="target">
	///	    An instance of type <typeparamref name="T"/> which should be validated.
	/// </param>
	/// <param name="errors">
	///		The operating <see cref="ValidationResult" /> to add validation entries to.
	/// </param>
	/// <returns>
	///	    The parameter <paramref name="errors"/>, for easier consumption.
	/// </returns>
	[SuppressMessage(
		"Design",
		"CA1000:Do not declare static members on generic types",
		Justification = "This is a static method to validate an instance of the self type."
	)]
	static abstract ValidationResult Validate(T? target, ValidationResult errors);
}
