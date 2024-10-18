using Immediate.Validations.Analyzers;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class ValidatorClassAnalyzerTests
{
	[Test]
	public async Task NonValidatorShouldNotWarn() =>
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

	[Test]
	public async Task CorrectlyDefinedValidatorShouldNotWarn1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public static bool ValidateProperty(int value, int operand) =>
					value > operand;

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task CorrectlyDefinedValidatorShouldNotWarn2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(int operand) : ValidatorAttribute
			{
				public int Operand { get; } = operand;

				public static bool ValidateProperty(int value, int operand) =>
					value > operand;

				public static string DefaultMessage => "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task CorrectlyDefinedValidatorShouldNotWarn3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				[TargetType]
				public required object Operand { get; init; }

				public static bool ValidateProperty<T>(T value, T operand) =>
					Comparer<T>.Default.Compare(value, operand) > 0;

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task CorrectlyDefinedValidatorShouldNotWarn4() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(
				[TargetType]
				object operand
			) : ValidatorAttribute
			{
				public object Operand { get; } = operand;

				public static bool ValidateProperty<T>(T value, T operand) =>
					Comparer<T>.Default.Compare(value, operand) > 0;

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task CorrectlyDefinedValidatorShouldNotWarn5() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(
				[TargetType]
				object operand
			) : ValidatorAttribute
			{
				public object Operand { get; } = operand;

				public static bool ValidateProperty<T>(T value, int operand) =>
					Comparer<T>.Default.Compare(value, (T)(object)operand) > 0;

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task CorrectlyDefinedValidatorShouldNotWarn6() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(
				params string[] operand
			) : ValidatorAttribute
			{
				public object Operand { get; } = operand;

				public static bool ValidateProperty(string value, params string[] operand) =>
					Comparer<string>.Default.Compare(value, operand[0]) > 0;

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task CorrectlyDefinedValidatorShouldNotWarn7() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(
				[TargetType]
				params object[] operand
			) : ValidatorAttribute
			{
				public object Operand { get; } = operand;

				public static bool ValidateProperty<T>(T value, params T[] operand) =>
					Comparer<T>.Default.Compare(value, operand[0]) > 0;

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task MissingValidateMethodShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class {|IV0001:GreaterThanAttribute|} : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task InstanceValidateMethodShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public bool {|IV0002:ValidateProperty|}(int value, int operand) =>
					value > operand;

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task DuplicateValidateMethodsShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }
			
				public static bool {|IV0003:ValidateProperty|}(int value)
				{
					return value > 0;
				}

				public static bool {|IV0003:ValidateProperty|}(int value, int operand)
				{
					return value > operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodIncorrectReturnShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public static int {|IV0004:ValidateProperty|}(int value, int operand)
				{
					return value;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodMissingParameterFromPropertyShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int {|IV0005:Operand|} { get; init; }

				public static bool ValidateProperty(int value)
				{
					return value > -1;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodMissingParameterFromParameterShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(
				int {|IV0005:operand|}
			) : ValidatorAttribute
			{
				public static bool ValidateProperty(int value)
				{
					return value > -1;
				}
			
				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodExtraParameterShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public static bool ValidateProperty(int value, int {|IV0006:operand|})
				{
					return value > operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodGeneralParameterVarianceFromPropertyShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int {|IV0005:Alpha|} { get; init; }
				public required int {|IV0005:Charlie|} { get; init; }
				public required int {|IV0005:Echo|} { get; init; }

				public static bool ValidateProperty(
					int value, 
					int {|IV0006:bravo|},
					int {|IV0006:delta|},
					int {|IV0006:foxtrot|}
				)
				{
					return value > 0;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodGeneralParameterVarianceFromParametersShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(
				int {|IV0005:alpha|},
				int {|IV0005:charlie|},
				int {|IV0005:echo|}
			): ValidatorAttribute
			{
				public static bool ValidateProperty(
					int value, 
					int {|IV0006:bravo|},
					int {|IV0006:delta|},
					int {|IV0006:foxtrot|}
				)
				{
					return value > 0;
				}
			
				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodMismatchTypesShouldWarn1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int? {|IV0007:Operand|} { get; init; }

				public static bool ValidateProperty(int value, int {|IV0007:operand|})
				{
					return value > operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodMismatchTypesShouldWarn2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class EqualToAttribute : ValidatorAttribute
			{
				public required string? {|IV0007:Operand|} { get; init; }

				public static bool ValidateProperty(string value, string {|IV0007:operand|})
				{
					return value == operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodMismatchTypesShouldWarn3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute(int? {|IV0007:operand|}) : ValidatorAttribute
			{
				public static bool ValidateProperty(int value, int {|IV0007:operand|})
				{
					return value > operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodMismatchTypesShouldWarn4() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class EqualToAttribute(string? {|IV0007:operand|}) : ValidatorAttribute
			{
				public static bool ValidateProperty(string value, string {|IV0007:operand|})
				{
					return value == operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidateMethodMismatchTypesShouldWarn5() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class EqualToAttribute(params string?[] {|IV0007:operand|}) : ValidatorAttribute
			{
				public static bool ValidateProperty(string value, params string[] {|IV0007:operand|})
				{
					return value == operand[0];
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidatePropertyMissingRequiredShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public int {|IV0008:Operand|} { get; init; }

				public static bool ValidateProperty(int value, int operand)
				{
					return value > operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidatorMultipleConstructors() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public {|IV0009:GreaterThanAttribute|}(int operand) { }
				public {|IV0009:GreaterThanAttribute|}(string operand) { }

				public static bool ValidateProperty(int value, int {|IV0006:operand|})
				{
					return value > operand;
				}

				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidatorWithNoDefaultMessageShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidatorClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;
			
			public sealed class {|IV0010:GreaterThanAttribute|} : ValidatorAttribute
			{
				public required int Operand { get; init; }
			
				public static bool ValidateProperty(int value, int operand) =>
					value > operand;
			}
			"""
		).RunAsync();
}
