namespace Immediate.Validations.Generators;

public sealed record ValidationClass
{
	public required string? Namespace { get; init; }
	public required EquatableReadOnlyList<Class> OuterClasses { get; init; }
	public required Class Class { get; init; }
	public required EquatableReadOnlyList<ValidationProperty> Properties { get; init; }
}

public sealed record Class
{
	public required string Type { get; init; }
	public required string Name { get; init; }
}

public sealed record ValidationProperty
{
	public required string PropertyName { get; init; }
	public required string TypeFullName { get; init; }
	public required string ValidatorName { get; init; }
	public required bool IsGenericMethod { get; init; }
	public required bool IsValidationProperty { get; init; }
	public required EquatableReadOnlyList<string> Parameters { get; init; }
	public required string? Message { get; init; }
	public required EquatableReadOnlyList<ValidationProperty> CollectionProperties { get; init; }
}

