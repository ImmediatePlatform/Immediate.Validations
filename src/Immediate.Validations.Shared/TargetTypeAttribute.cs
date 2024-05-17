namespace Immediate.Validations.Shared;

/// <summary>
///	    Added to an <c><see cref="object"/></c> property or parameter on a <see cref="ValidatorAttribute"/> to indicate
///	    that the value used for the property/parameter should have the same type as the validated property.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class TargetTypeAttribute : Attribute;
