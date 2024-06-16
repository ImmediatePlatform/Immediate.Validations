using Immediate.Validations.Analyzers;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class MissingAdditionalValidationsMethodAnalyzerTests
{
	[Fact]
	public async Task UnmarkedClassIsIgnored() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingAdditionalValidationsMethodAnalyzer>(
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
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingAdditionalValidationsMethodAnalyzer>(
			"""
			using System.Diagnostics.CodeAnalysis;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[SuppressMessage("", "")]
				public required int Unrelated { get; init; }

				public static ValidationResult Validate(Target target) => [];

				private static void AdditionalValidations(ValidationResult errors, Target target) { }
			}
			"""
		).RunAsync();

	[Fact]
	public async Task MissingMethodShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingAdditionalValidationsMethodAnalyzer>(
			"""
			using System.Diagnostics.CodeAnalysis;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record {|IV0017:Target|} : IValidationTarget<Target>
			{
				[SuppressMessage("", "")]
				public required int Unrelated { get; init; }

				public static ValidationResult Validate(Target target) => [];
			}
			"""
		).RunAsync();
}
