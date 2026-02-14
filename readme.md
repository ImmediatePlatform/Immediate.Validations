# Immediate.Validations

[![NuGet](https://img.shields.io/nuget/v/Immediate.Validations.svg?style=plastic)](https://www.nuget.org/packages/Immediate.Validations/)
[![GitHub release](https://img.shields.io/github/release/ImmediatePlatform/Immediate.Validations.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Validations/releases/)
[![GitHub license](https://img.shields.io/github/license/ImmediatePlatform/Immediate.Validations.svg)](https://github.com/ImmediatePlatform/Immediate.Validations/blob/master/license.txt) 
[![GitHub issues](https://img.shields.io/github/issues/ImmediatePlatform/Immediate.Validations.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Validations/issues/) 
[![GitHub issues-closed](https://img.shields.io/github/issues-closed/ImmediatePlatform/Immediate.Validations.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Validations/issues?q=is%3Aissue+is%3Aclosed) 
[![GitHub Actions](https://github.com/ImmediatePlatform/Immediate.Validations/actions/workflows/build.yml/badge.svg)](https://github.com/ImmediatePlatform/Immediate.Validations/actions)
---

Immediate.Validations is a source generator validating
[`Immediate.Handlers`](https://github.com/ImmediatePlatform/Immediate.Handlers) handler parameters.

## Installing Immediate.Validations

You can install [Immediate.Validations with NuGet](https://www.nuget.org/packages/Immediate.Validations):

    Install-Package Immediate.Validations
    
Or via the .NET Core command line interface:

    dotnet add package Immediate.Validations

Either command, from Package Manager Console or .NET Core CLI, will download and install Immediate.Validations.

### Creating Validation Classes

Indicate that a class should be validated by adding the `[Validate]` attribute and `IValidationTarget<>` interface:

```cs
[Validate]
public partial record Query : IValidationTarget<Query>;
```

When Nullable Reference Types is enabled, any non-nullable reference types are automatically checked for `null`. Other
validations are available like so:

```cs
[Validate]
public partial record Query : IValidationTarget<Query>
{
	[GreaterThan(0)]
	public required int Id { get; init; }
}
```

### Referencing Other Properties

Since attributes cannot reference anything other than constant strings, the way to reference static and instance
properties, fields, and methods is to use the `nameof()` to identify which property, field, or method should be used. Example:

```cs
[Validate]
public partial record Query : IValidationTarget<Query>
{
	[GeneratedRegex(@"^\d+$")]
	private static partial Regex AllDigitsRegex();

	[Match(regex: nameof(AllDigitsRegex))]
	public required string Id { get; init; }
}
```

### Custom Messages

Provide a custom message to any validation using the `Message` property of the attribute. This message will be parsed
for template parameters, which will be applied to the message before rendering to the validation result. The target property
name is available as `{PropertyName}`, and it's value via `{PropertyValue}`. 

Other parameter values will be added using their property name suffixed with `Value` (for example, the
`GreaterThanAttribute` uses a `comparison` parameter, so the value is available via `ComparisonValue`). If another
property on the target class is referenced via `nameof(Property)`, the name of that property will be available using the
`Name` suffix (for example, `ComparisonName` for the `comparison` property).

```cs
[Validate]
public partial record Query : IValidationTarget<Query>
{
	[GreaterThan(0, Message = "'{PropertyName}' must be greater than '{ComparisonValue}'")]
	public required int Id { get; init; }
}
```

### Extending Validation Classes

If attributes are not enough to specify how to validate a class, an `AdditionalValidations` method can be used to write
additional validations for the class.

```cs
[Validate]
public partial record Query : IValidationTarget<Query>
{
	public required bool Enabled { get; init; }
	public required int Id { get; init; }

	private static void AdditionalValidations(
		ValidationResult errors,
		Query target
	)
	{
		if (target.Enabled)
		{
			// Use a lambda to use the default message or override message;
			// the message will be templated in the same way as attribute validations.
			errors.Add(
				() => GreaterThanAttribute.ValidateProperty(
					target.Id,
					0
				)
			);
		}

		if (false)
		{
			// Manually create a `ValidationError` and add it to the `ValidationResult`.
			errors.Add(
				new ValidationError()
				{
					PropertyName = "ExampleProperty",
					ErrorMessage = "Example Message",
				}
			)
		}
	}
}
```

## Using Immediate.Validations

### Validating instances directly

An instance of an IV object can be validated directly by calling `ClassName.Validate(instance);`. In the example above,
this would look like:

```cs
public void Method(Query query)
{
	var results = Query.Validate(query);
}
```

The `results` object will contain information on whether the instance is valid, and if not, the validation failures that
occurred during validation.

If you would like to throw an exception if the validation fails, you can call `ValidationException.ThrowIfInvalid()`. This
method can be called using the `ValidationResult` returned from validation if you have already validated it, or using an
instance of the object if you would like to validate and throw. Examples:

```cs
public void Method(Query query)
{
	var results = Query.Validate(query);
	ValidationException.ThrowIfInvalid(results);

	// or

	ValidationException.ThrowIfInvalid(query);
}
```

### Using Immediate.Validations with [Immediate.Handlers](https://github.com/ImmediatePlatform/Immediate.Handlers)

Adding code to explicitly validate the object in every method that needs to do so can be annoying. Immediate.Validations
is intended to integrate primarily with Immediate.Handlers, and so it provides an IH `Behavior<,>` which can be used to
automatically add validation to every IH handler which has a request which is an `IValidation<>` object. Add
Immediate.Validations to the Immediate.Handlers behaviors pipeline by including it in the list of default Behaviors for
the assembly:

```cs
using Immediate.Validations.Shared;

[assembly: Behaviors(
	typeof(ValidationBehavior<,>)
)]
```

### Using Immediate.Validations in an ASP.NET Core application

The result of doing the above is that when a parameter fails one or more validations, a `ValidationException` is thrown,
which can be handled via ProblemDetails or any other infrastructure mechanism.

Example using ProblemDetails:
```cs
builder.Services.AddProblemDetails(ConfigureProblemDetails);

public static void ConfigureProblemDetails(ProblemDetailsOptions options) =>
	options.CustomizeProblemDetails = c =>
	{
		if (c.Exception is null)
			return;

		c.ProblemDetails = c.Exception switch
		{
			ValidationException ex => new ValidationProblemDetails(
				ex
					.Errors
					.GroupBy(x => x.PropertyName, StringComparer.OrdinalIgnoreCase)
					.ToDictionary(
						x => x.Key,
						x => x.Select(x => x.ErrorMessage).ToArray(),
						StringComparer.OrdinalIgnoreCase
					)
			)
			{
				Status = StatusCodes.Status400BadRequest,
			},

			// other exception handling as desired

			var ex => new ProblemDetails
			{
				Detail = "An error has occurred.",
				Status = StatusCodes.Status500InternalServerError,
			},
		};

		c.HttpContext.Response.StatusCode =
			c.ProblemDetails.Status
			?? StatusCodes.Status500InternalServerError;
	};

```
