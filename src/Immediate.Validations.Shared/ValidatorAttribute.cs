namespace Immediate.Validations.Shared;

/// <summary>
///	    Applied to a property to indicate the property should be validated.
/// </summary>
/// <remarks>
/// <para>
///	    A properly defined subclass of <see cref="ValidatorAttribute"/> must have a <c><see langword="static"/> (<see
///	    langword="bool"/> Invalid, <see langword="string"/>? DefaultMessage) ValidateProperty(? value)</c> method, which
///	    will be used to validate properties.
/// </para>
/// <para>
///	    The type of the first parameter must match the type of the property. If it does not, then the validation will
///	    not be generated and a compiler diagnostic will be generated instead.
/// </para>
/// <para>
///	    If the method is generic and type constraints are applied to the generic parameter, then the method can be used
///	    for any type that satisfies the constraints of the parameter.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public abstract class ValidatorAttribute : Attribute
{
	/// <summary>
	///		A custom error message to represent the specific validation failure.
	/// </summary>
	public string? Message { get; init; }
}
