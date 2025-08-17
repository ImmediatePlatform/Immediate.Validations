using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis.Testing;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class InvalidAttributeTargetSuppressorTests
{
	public static readonly DiagnosticResult CS0658 =
		DiagnosticResult.CompilerWarning("CS0658");

	[Test]
	public async Task ElementAttributeInValidatorIsSuppressed() =>
		await AnalyzerTestHelpers
			.CreateSuppressorTest<InvalidAttributeTargetSuppressor>(
				"""
				#nullable enable

				using System.Collections.Generic;
				using Immediate.Validations.Shared;
			
				[Validate]
				public class Target : IValidationTarget<Target>
				{
					[{|#0:element|}: MaxLength(3)]
					public required List<string> Strings { get; init; }
				
					public ValidationResult Validate() => [];
					public ValidationResult Validate(ValidationResult errors) => [];
					public static ValidationResult Validate(Target target) => [];
					public static ValidationResult Validate(Target target, ValidationResult errors) => [];
				}
				"""
			)
			.WithSpecificDiagnostics([CS0658])
			.WithExpectedDiagnosticsResults([
				CS0658.WithLocation(0).WithIsSuppressed(true),
			])
			.RunAsync();

	[Test]
	public async Task ElementAttributeNotInValidatorIsNotSuppressed() =>
		await AnalyzerTestHelpers
			.CreateSuppressorTest<InvalidAttributeTargetSuppressor>(
				"""
				#nullable enable

				using System.Collections.Generic;
				using Immediate.Validations.Shared;
			
				public class Target
				{
					[{|#0:element|}: MaxLength(3)]
					public required List<string> Strings { get; init; }
				
					public ValidationResult Validate() => [];
					public ValidationResult Validate(ValidationResult errors) => [];
					public static ValidationResult Validate(Target target) => [];
					public static ValidationResult Validate(Target target, ValidationResult errors) => [];
				}
				"""
			)
			.WithSpecificDiagnostics([CS0658])
			.WithExpectedDiagnosticsResults([
				CS0658.WithLocation(0).WithIsSuppressed(false),
			])
			.RunAsync();

	[Test]
	public async Task NonElementAttributeInValidatorIsNotSuppressed() =>
		await AnalyzerTestHelpers
			.CreateSuppressorTest<InvalidAttributeTargetSuppressor>(
				"""
				#nullable enable

				using System.Collections.Generic;
				using Immediate.Validations.Shared;
			
				[Validate]
				public class Target : IValidationTarget<Target>
				{
					[{|#0:foo|}: MaxLength(3)]
					public required List<string> Strings { get; init; }
				
					public ValidationResult Validate() => [];
					public ValidationResult Validate(ValidationResult errors) => [];
					public static ValidationResult Validate(Target target) => [];
					public static ValidationResult Validate(Target target, ValidationResult errors) => [];
				}
				"""
			)
			.WithSpecificDiagnostics([CS0658])
			.WithExpectedDiagnosticsResults([
				CS0658.WithLocation(0).WithIsSuppressed(false),
			])
			.RunAsync();

	[Test]
	public async Task ElementAttributeInValidatorOnMethodIsNotSuppressed() =>
		await AnalyzerTestHelpers
			.CreateSuppressorTest<InvalidAttributeTargetSuppressor>(
				"""
				#nullable enable

				using System.Runtime.CompilerServices;
				using Immediate.Validations.Shared;
			
				[Validate]
				public class Target : IValidationTarget<Target>
				{
					[{|#0:element|}: MethodImpl(MethodImplOptions.NoInlining)]
					public void DoSomething() { }
				
					public ValidationResult Validate() => [];
					public ValidationResult Validate(ValidationResult errors) => [];
					public static ValidationResult Validate(Target target) => [];
					public static ValidationResult Validate(Target target, ValidationResult errors) => [];
				}
				"""
			)
			.WithSpecificDiagnostics([CS0658])
			.WithExpectedDiagnosticsResults([
				CS0658.WithLocation(0).WithIsSuppressed(false),
			])
			.RunAsync();

	[Test]
	public async Task ElementAttributeInValidatorNonCollectionIsNotSuppressed() =>
		await AnalyzerTestHelpers
			.CreateSuppressorTest<InvalidAttributeTargetSuppressor>(
				"""
				#nullable enable

				using System.Collections.Generic;
				using Immediate.Validations.Shared;
			
				[Validate]
				public class Target : IValidationTarget<Target>
				{
					[{|#0:element|}: MaxLength(3)]
					public required IEnumerable<string> Strings { get; init; }
				
					public ValidationResult Validate() => [];
					public ValidationResult Validate(ValidationResult errors) => [];
					public static ValidationResult Validate(Target target) => [];
					public static ValidationResult Validate(Target target, ValidationResult errors) => [];
				}
				"""
			)
			.WithSpecificDiagnostics([CS0658])
			.WithExpectedDiagnosticsResults([
				CS0658.WithLocation(0).WithIsSuppressed(false),
			])
			.RunAsync();
}
