using Immediate.Validations.Analyzers;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class ValidatorClassAnalyzerTests
{
	[Fact]
	public async Task NonValdiatorShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute
			{
				public static void ValidateProperty(int value, int operand)
				{
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidateMethodPresentShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value, int operand)
				{
					return value <= operand
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task MissingValidateMethodShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class {|IV0001:GreaterThanAttribute|} : ValidatorAttribute
			{
				public required int Operand { get; init; }
			}
			"""
		).RunAsync();

	[Fact]
	public async Task InstanceValidateMethodShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public (bool Invalid, string? DefaultMessage) {|IV0002:ValidateProperty|}(int value, int operand)
				{
					return value <= operand
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task DuplicateValidateMethodsShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }
			
				public static (bool Invalid, string? DefaultMessage) {|IV0003:ValidateProperty|}(int value)
				{
					return value <= 0
						? (true, "Property must not be `null`.")
						: default;
				}
			
				public static (bool Invalid, string? DefaultMessage) {|IV0003:ValidateProperty|}(int value, int operand)
				{
					return value <= operand
						? (true, "Property must not be `null`.")
						: default;
				}
						}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidateMethodIncorrectReturnShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public static bool {|IV0004:ValidateProperty|}(int value, int operand)
				{
					return value <= operand;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidateMethodMissingParameterShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int {|IV0005:Operand|} { get; init; }

				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value)
				{
					return value <= -1
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidateMethodExtraParameterShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value, int {|IV0006:operand|})
				{
					return value <= operand
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidateMethodGeneralParameterVarianceShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int {|IV0005:Alpha|} { get; init; }
				public required int {|IV0005:Charlie|} { get; init; }
				public required int {|IV0005:Echo|} { get; init; }

				public static (bool Invalid, string? DefaultMessage) ValidateProperty(
					int value, 
					int {|IV0006:bravo|},
					int {|IV0006:delta|},
					int {|IV0006:foxtrot|}
				)
				{
					return value <= 0
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidateMethodMismatchTypesShouldWarn1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int? {|IV0007:Operand|} { get; init; }

				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value, int {|IV0007:operand|})
				{
					return value <= operand
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidateMethodMismatchTypesShouldWarn2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class EqualToAttribute : ValidatorAttribute
			{
				public required string? {|IV0007:Operand|} { get; init; }

				public static (bool Invalid, string? DefaultMessage) ValidateProperty(string value, string {|IV0007:operand|})
				{
					return value != operand
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ValidatePropertyMissingRequiredShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public int {|IV0008:Operand|} { get; init; }

				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value, int operand)
				{
					return value <= operand
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();

}
