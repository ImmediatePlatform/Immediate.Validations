using Immediate.Validations.Analyzers;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class ValidateClassAnalyzerTests
{
	[Fact]
	public async Task UnmarkedClassIsIgnored() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed record Target
			{
				[NotNull]
				public required int Id { get; init; }
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidTargetShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task MissingValidateAttributeShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			public sealed partial record {|IV0011:Target|} : IValidationTarget<Target>
			{
				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task MissingIValidationTargetShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record {|IV0012:Target|}
			{
				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[NotNull]
				public required string Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0013:NotNull|}]
				public required int Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				public enum ExampleEnum { None = 0, Value = 1 }

				[EnumValue]
				public required ExampleEnum Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0013:EnumValue|}]
				public required int Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[NotEmptyOrWhiteSpace]
				public required string Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0013:NotEmptyOrWhiteSpace|}]
				public required int Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn4() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[NotEmptyOrWhiteSpace]
				public required List<string> Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn4() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0013:NotEmptyOrWhiteSpace|}]
				public required List<int> Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn5() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				public enum ExampleEnum { None = 0, Value = 1 }

				[EnumValue]
				public required List<ExampleEnum> Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn5() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0013:EnumValue|}]
				public required List<int> Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();
}
