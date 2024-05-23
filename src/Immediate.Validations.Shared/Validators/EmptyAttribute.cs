using System.Collections;

namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should be empty.
/// </summary>
public sealed class EmptyAttribute : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the given <paramref name="value"/> is properly defined.
	/// </summary>
	/// <param name="value">
	///	    The value to validate.
	/// </param>
	/// <returns>
	///	    A <see cref="ValueTuple{T1, T2}"/> indicating whether the property is valid or not, along with an error
	///     message if the property is not valid.
	/// </returns>
	public static (bool Invalid, string? DefaultMessage) ValidateProperty<T>(T value) =>
		IsEmpty(value)
			? default
			: (true, "Property must be empty.");

	internal static bool IsEmpty<T>(T value) =>
		value switch
		{
			null => true,
			string s when string.IsNullOrWhiteSpace(s) => true,
			ICollection { Count: 0 } => true,
			_ => EqualityComparer<T>.Default.Equals(value, default)
		};

	private static bool IsEmpty(IEnumerable enumerable)
	{
		var enumerator = enumerable.GetEnumerator();

		using (enumerator as IDisposable)
			return !enumerator.MoveNext();
	}
}
