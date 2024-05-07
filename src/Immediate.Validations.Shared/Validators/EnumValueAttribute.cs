namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value of the property should be a defined value of the <see
///     langword="enum" />.
/// </summary>
public sealed class EnumValueAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <see langword="enum" /> <paramref name="value"/> is properly defined.
	/// </summary>
	/// <typeparam name="T">
	///	    The type of the provided <see langword="enum" /> value.
	/// </typeparam>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    A <see cref="ValueTuple{T1, T2}"/> indicating whether the property is valid or not, along with an error
	///     message if the property is not valid.
	/// </returns>
	public static (bool Invalid, string? DefaultMessage) ValidateProperty<T>(T value)
		where T : struct, Enum
	{
		return !Enum.IsDefined(value)
			? (true, $"The value `{value}` is not defined in the enum type `{typeof(T).Name}`.")
			: default;
	}
}
