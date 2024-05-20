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

## Using Immediate.Validations

Add Immediate.Validations to the Immediate.Handlers behaviors pipeline by including it in the list of default Behaviors
for the assembly:

```cs
using Immediate.Validations.Shared;

[assembly: Behaviors(
	typeof(ValidationBehavior<,>)
)]
```

### Creating Validation Classes

Indicate that a class should be validated by adding the `[Validate]` attribute and `IValidationTarget<>` interface:

```cs
[Validate]
public record Query : IValidationTarget<Query>;
```

When Nullable Reference Types is enabled, any non-nullable reference types are automatically checked for `null`. Other
validations are available like so:

```cs
[Validate]
public record Query : IValidationTarget<Query>
{
	[GreaterThan(0)]
	public required int Id { get; init; }
}
```

### Results

The result of doing the above is that when a parameter fails one or more validations, a `ValidationException` is thrown,
which can be handled via ProblemDetails or any other infrastructure mechanism.
