namespace Immediate.Validations.Generators;

public sealed record ValidationTarget
{
	public required string? Namespace { get; init; }
	public required EquatableReadOnlyList<Class> OuterClasses { get; init; }
	public required Class Class { get; init; }
	public required bool HasAdditionalValidationsMethod { get; init; }
	public required bool IsReferenceType { get; init; }
	public required EquatableReadOnlyList<string> BaseValidationTargets { get; init; }
	public required EquatableReadOnlyList<ValidationTargetProperty> Properties { get; init; }
}

public sealed record Class
{
	public required string Type { get; init; }
	public required string Name { get; init; }
}

public sealed record ValidationTargetProperty
{
	public required string Name { get; init; }
	public required string PropertyName { get; init; }
	public required string TypeFullName { get; init; }
	public required bool IsReferenceType { get; init; }
	public required bool IsNullable { get; init; }
	public required bool IsValidationProperty { get; init; }
	public required string? ValidationTypeFullName { get; init; }
	public required ValidationTargetProperty? CollectionPropertyDetails { get; init; }
	public required EquatableReadOnlyList<PropertyValidation> Validations { get; init; }
	public required EquatableReadOnlyList<PropertyValidation> NullValidations { get; init; }
}

public sealed record PropertyValidation
{
	public required string ValidatorName { get; init; }
	public required bool IsGenericMethod { get; init; }
	public required bool IsNullable { get; init; }
	public required EquatableReadOnlyList<string> Parameters { get; init; }
	public required string? Message { get; init; }
}

