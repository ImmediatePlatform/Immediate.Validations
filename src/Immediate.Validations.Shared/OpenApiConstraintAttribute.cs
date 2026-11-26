namespace Immediate.Validations.Shared;

/// <summary>
///	    A schema constraint generated for a property of a validation target.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
public sealed class OpenApiConstraintAttribute(
	string propertyName,
	OpenApiConstraintKind kind,
	string? value = null,
	int depth = 0
) : Attribute
{
	/// <summary>The CLR property name.</summary>
	public string PropertyName { get; } = propertyName;

	/// <summary>The kind of schema constraint.</summary>
	public OpenApiConstraintKind Kind { get; } = kind;

	/// <summary>The constraint value, formatted using the invariant culture.</summary>
	public string? Value { get; } = value;

	/// <summary>The number of collection levels to descend from the property.</summary>
	public int Depth { get; } = depth;
}

/// <summary>The schema constraints generated from validation rules.</summary>
public enum OpenApiConstraintKind
{
	/// <summary>Disallows a null value.</summary>
	NotNull,
	/// <summary>Specifies an inclusive lower numeric bound.</summary>
	Minimum,
	/// <summary>Specifies an exclusive lower numeric bound.</summary>
	ExclusiveMinimum,
	/// <summary>Specifies an inclusive upper numeric bound.</summary>
	Maximum,
	/// <summary>Specifies an exclusive upper numeric bound.</summary>
	ExclusiveMaximum,
	/// <summary>Specifies the minimum string length.</summary>
	MinLength,
	/// <summary>Specifies the maximum string length.</summary>
	MaxLength,
	/// <summary>Specifies the minimum array length.</summary>
	MinItems,
	/// <summary>Specifies the maximum array length.</summary>
	MaxItems,
}
