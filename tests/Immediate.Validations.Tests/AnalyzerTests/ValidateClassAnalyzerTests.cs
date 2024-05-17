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
			using System.Diagnostics.CodeAnalysis;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[SuppressMessage("", "")]
				public required int Unrelated { get; init; }

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
			
			public sealed partial record {|IV0012:Target|} : IValidationTarget<Target>
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
			public sealed partial record {|IV0013:Target|}
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
				[{|IV0014:NotNull|}]
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
				[{|IV0014:EnumValue|}]
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
				[{|IV0014:NotEmptyOrWhiteSpace|}]
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
				[{|IV0014:NotEmptyOrWhiteSpace|}]
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
				[{|IV0014:EnumValue|}]
				public required List<int> Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn6() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[NotEmptyOrWhiteSpace]
				public required string[] Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn6() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0014:NotEmptyOrWhiteSpace|}]
				public required int[] Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn7() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Equal(0)]
				public required int Id { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn7() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Equal({|IV0015:"test"|})]
				public required int Id { get; init; }
			
				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidValidatorTypeShouldNotWarn8() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Equal(nameof(KeyValue))]
				public required int Id { get; init; }
				public required int KeyValue { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InvalidValidatorTypeShouldWarn8() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Equal({|IV0016:nameof(KeyValue)|})]
				public required int Id { get; init; }
				public required string KeyValue { get; init; }
						
				public static List<ValidationError> Validate(Target target) => [];
			}
			"""
		).RunAsync();
}
