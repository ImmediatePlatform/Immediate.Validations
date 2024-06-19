# Immediate.Validations.Analyzers

## IV0001: Validators must have a valid `ValidateProperty` method

A Validator that inherits from `ValidatorAttribute` must have a `public static bool ValidateProperty()` in order to
validate an attached property.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0002: `ValidateProperty` method must be static

The `ValidateProperty()` for a particular validator must be `static`.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0003: `ValidateProperty` method must be unique

IV only supports a single `public static void ValidateProperty()` in the validator class.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0004: `ValidateProperty` method must have a valid return

The `ValidateProperty()` for a particular validator must return a `bool`. A `true` should represent a successful
validation, while a `false` represents an invalid property.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0005: `ValidateProperty` method is missing parameters

The validator class has a property or constructor parameter that does not have a matching parameter on the
`ValidateProperty()` method. All properties or constructor parameters should have a matching parameter, which can be
used provide attribute values to the `ValidateProperty()` method.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0006: `ValidateProperty` method has extra parameters

The `ValidatorProperty()` method has a parameter that does not have a matching property or constructor parameter. All
properties or constructor parameters should have a matching parameter, which can be used provide attribute values to the
`ValidateProperty()` method.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0007: `ValidateProperty` parameters and Validator properties must match

The type of the constructor parameter or the property must be assignable to the parameter.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0008: Validator property must be `required`

The property or the constructor parameter must be required if the `ValidateProperty()` parameter is required.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0009: Validator has too many constructors

IV only supports a single constructor.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0011: Assembly-wide `Behaviors` attribute should use `ValidationBehavior<,>`

The `ValidationBehavior<,>` behavior should be registered as part of the assembly-wide Immediate.Handlers pipeline. This
will ensure that all request types that should be validated are validated as part of the IH command handling.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Warning              |
| CodeFix  | False                |

## IV0012: Validation targets must be marked `[Validate]`

The `[Validate]` attribute is necessary for the IV Source Generator to operate, and must be applied to a type in order
to implement `IValidationTarget<T>` and use the validator attributes attached to any properties.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0013: Validation targets should implement the interface `IValidationTarget<>`

The `IValidationTarget<>` interface is necessary for the `ValidationBehavior<,>` behavior to be enabled and validate the
class when part of an IH command handler.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Warning              |
| CodeFix  | False                |

## IV0014: Validator will not be used

A validator can only be used on properties that have compatible types. For example, the `[Length]` validator can only be
used on `string`s.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Warning              |
| CodeFix  |                      |

## IV0015: Parameter is incompatible type

A value of invalid type was provided to the validator.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Warning              |
| CodeFix  | False                |

## IV0016: Parameter is incompatible type

A `nameof()` reference of invalid type was provided to the validator.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Warning              |
| CodeFix  | False                |

## IV0018: nameof() target is invalid

An invalid `nameof()` destination was used. Only immediate properties, fields, and methods are allowed to be used as a
`nameof()` value for the validator.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |

## IV0019: Validator is missing `DefaultMessage`

A Validator that inherits from `ValidatorAttribute` must have a `public static string DefaultMessage => "";` or `public
const string DefaultMessage = "";` to provide a default validation message when the property is invalid.

| Item     | Value                |
|----------|----------------------|
| Category | ImmediateValidations |
| Enabled  | True                 |
| Severity | Error                |
| CodeFix  | False                |
