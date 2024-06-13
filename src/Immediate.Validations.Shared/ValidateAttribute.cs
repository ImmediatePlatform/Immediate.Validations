namespace Immediate.Validations.Shared;

/// <summary>
///		Applied to a class to indicate that validation methods should be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public sealed class ValidateAttribute : Attribute;
