namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate that the value should one of the provided <paramref name="values"/>.
/// </summary>
/// <param name="values">
///		An array of values which the applied property should be.
/// </param>
public sealed class OneOfAttribute(
	[TargetType]
	params object[] values
) : ValidatorAttribute
{
	/// <summary>
	///	    Validates that the property value is one of the specified <paramref name="values"/>.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="values"></param>
	/// <returns></returns>
	public static (bool Invalid, string? DefaultMessage) ValidateProperty<T>(T target, params T[] values) =>
		values.Contains(target)
			? default
			: (true, $"Value `{target}` was not one of the specified values: {string.Join(", ", values)}.");
}
