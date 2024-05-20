namespace Immediate.Validations.Shared;

/// <summary>
///		Applied to a class to indicate that validation methods should be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ValidateAttribute : Attribute;
