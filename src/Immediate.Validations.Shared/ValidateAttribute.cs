namespace Immediate.Validations.Shared;

/// <summary>
///		Applied to a class to indicate that validation methods should be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public sealed class ValidateAttribute : Attribute
{
	/// <summary>
	///		Set to <see langword="true" /> in order to skip validating local properties.
	/// </summary>
	/// <remarks>
	///		Base class and interface validators will still be called.
	/// </remarks>
	public bool SkipSelf { get; init; }
}
