namespace Immediate.Validations.Shared.Attributes;

/// <summary>
///	    Applied to a property to indicate that the property should not be <see langword="null"/>.
/// </summary>
public sealed class NotNullAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <paramref name="value"/> is not <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">
	///	    The type of the provided value.
	/// </typeparam>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    A <see cref="ValueTuple{T1, T2}"/> indicating whether the property is valid or not, along with an error
	///     message if the property is not valid.
	/// </returns>
	public static (bool Invalid, string? DefaultMessage) ValidateProperty<T>(T value)
		where T : class
	{
		return value is null
			? (true, "Property must not be `null`.")
			: default;
	}
}
