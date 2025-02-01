using Immediate.Validations.Analyzers;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class ValidateClassAnalyzerTests
{
	[Test]
	public async Task UnmarkedClassIsIgnored() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed record Target
			{
				public required int Id { get; init; }
			}
			"""
		).RunAsync();

	[Test]
	public async Task UnmarkedClassWithValidatedPropertiesShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using Immediate.Validations.Shared;

			public sealed record {|IV0012:{|IV0013:Target|}|}
			{
				[NotEmpty]
				public required int Id { get; init; }
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidTargetShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Diagnostics.CodeAnalysis;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				public required int Unrelated { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task MissingValidateAttributeShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			public sealed partial record {|IV0012:Target|} : IValidationTarget<Target>
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task MissingIValidationTargetShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record {|IV0013:Target|}
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class NotNullClassAttribute : ValidatorAttribute
			{
				public static bool ValidateProperty<T>(T value)
					where T : class =>
					value is not null;
			
				public static string DefaultMessage => ValidationConfiguration.Localizer[nameof(NotNullAttribute)].Value;
			}
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0014:NotNullClass|}]
				public required int Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[MaxLength(3)]
				public required string Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0014:MaxLength(3)|}]
				public required int Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn4() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[MaxLength(3)]
				public required List<string> Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn4() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0014:MaxLength(3)|}]
				public required List<int> Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn6() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[MaxLength(3)]
				public required string[] Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn6() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0014:MaxLength(3)|}]
				public required int[] Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
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

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn9() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[MaxLength(0)]
				public required string Id { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn9() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[MaxLength({|IV0015:"test"|})]
				public required string Id { get; init; }
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn10() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[MaxLength(nameof(KeyValue))]
				public required string Id { get; init; }
				public required int KeyValue { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn10() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[MaxLength({|IV0016:nameof(KeyValue)|})]
				public required string Id { get; init; }
				public required string KeyValue { get; init; }
						
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn11() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] object first,
				string second
			) : ValidatorAttribute
			{
				public required string Third { get; init; }

				public static bool ValidateProperty(
					string target, 
					string first,
					string second,
					string third,
					string fourth = "fourth"
				) =>
					target == $"{first}-{second}-{third}-{fourth}";

				public const string DefaultMessage = "";
			}
						
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy(first: nameof(FirstValue), "Hello World", Third = "Value", Message = "What's going on?")]
				public required string Id { get; init; }
				public required string FirstValue { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn11() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] object first,
				string second
			) : ValidatorAttribute
			{
				public required string Third { get; init; }

				public static bool ValidateProperty(
					string target, 
					string first,
					string second,
					string third,
					string fourth = "fourth"
				) =>
					target == $"{first}-{second}-{third}-{fourth}";

				public const string DefaultMessage = "";
			}
						
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy(
					{|IV0016:first: nameof(FirstValue)|}, 
					"Hello World", 
					Third = "Value", 
					Message = "What's going on?"
				)]
				public required string Id { get; init; }

				public required int FirstValue { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn12() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty(
					string target, 
					params string[] first
				) =>
					target == first[0];

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", "456", nameof(FirstValue), Message = "What's going on?")]
				public required string Id { get; init; }
				public required string FirstValue { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn12() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty(
					string target, 
					params string[] first
				) =>
					target == first[0];

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", {|IV0015:456|}, {|IV0016:nameof(FirstValue)|}, Message = "What's going on?")]
				public required string Id { get; init; }
				public required int FirstValue { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn13() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty<T>(
					T target, 
					params T[] first
				) =>
					EqualityComparer<T>.Default.Equals(target, first[0]);

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", "456", nameof(FirstValue), Message = "What's going on?")]
				public required string Id { get; init; }
				public required string FirstValue { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn13() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty<T>(
					T target, 
					params T[] first
				) =>
					EqualityComparer<T>.Default.Equals(target, first[0]);

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", {|IV0015:456|}, {|IV0016:nameof(FirstValue)|}, Message = "What's going on?")]
				public required string Id { get; init; }
				public required int FirstValue { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn14() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty(
					string target, 
					params string[] first
				) =>
					target == first[0];

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", "456", nameof(FirstValue), Message = "What's going on?")]
				public required string Id { get; init; }
				public string FirstValue() => "Hello World!";

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn14() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty(
					string target, 
					params string[] first
				) =>
					target == first[0];

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", {|IV0015:456|}, {|IV0016:nameof(FirstValue)|}, Message = "What's going on?")]
				public required string Id { get; init; }
				public int FirstValue() => 123;
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn15() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty<T>(
					T target, 
					params T[] first
				) =>
					EqualityComparer<T>.Default.Equals(target, first[0]);

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", "456", nameof(FirstValue), Message = "What's going on?")]
				public required string Id { get; init; }
				public static string FirstValue() => "Hello World!";

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn15() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] params object[] first
			) : ValidatorAttribute
			{
				public static bool ValidateProperty<T>(
					T target, 
					params T[] first
				) =>
					EqualityComparer<T>.Default.Equals(target, first[0]);

				public const string DefaultMessage = "";
			}

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy("123", {|IV0015:456|}, {|IV0016:nameof(FirstValue)|}, Message = "What's going on?")]
				public required string Id { get; init; }
				public static int FirstValue() => 123;
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidValidatorTypeShouldNotWarn16() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[OneOf(nameof(Values))]
				public required string Id { get; init; }

				private static readonly string[] Values = ["123", "456", "789"];

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidValidatorTypeShouldWarn16() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[OneOf({|IV0016:nameof(Values)|})]
				public required int Id { get; init; }

				private static readonly string[] Values = ["123", "456", "789"];

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task InvalidNameofShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Equal({|IV0017:nameof(DateTime)|})]
				public required string Id { get; init; }
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidNameofInheritedClassShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class BaseClass : IValidationTarget<BaseClass>
			{
				public required int ValueA { get; init; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(BaseClass target) => [];
				public static ValidationResult Validate(BaseClass target, ValidationResult errors) => [];
			}

			[Validate]
			public partial class SubClass : BaseClass, IValidationTarget<SubClass>
			{
				[Equal(nameof(ValueA))]
				public required int ValueB { get; init; }
		
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(SubClass target) => [];
				public static ValidationResult Validate(SubClass target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task ValidNameofInheritedInterfaceShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial interface IBaseInterface : IValidationTarget<IBaseInterface>
			{
				int ValueA { get; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				static ValidationResult IValidationTarget<IBaseInterface>.Validate(IBaseInterface target) => [];
				static ValidationResult IValidationTarget<IBaseInterface>.Validate(IBaseInterface target, ValidationResult errors) => [];
			}

			[Validate]
			public partial interface IInterface : IBaseInterface, IValidationTarget<IInterface>
			{
				[Equal(nameof(ValueA))]
				int ValueB { get; }

				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				static ValidationResult IValidationTarget<IInterface>.Validate(IInterface target) => [];
				static ValidationResult IValidationTarget<IInterface>.Validate(IInterface target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task NotNullOnIntShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[{|IV0018:NotNull|}]
				public required int IntProperty { get; init; }
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task NotNullOnNullableIntShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[NotNull]
				public required int? IntProperty { get; init; }
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();

	[Test]
	public async Task NotNullOnStringShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ValidateClassAnalyzer>(
			"""
			using System;
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[NotNull]
				public required string StringProperty { get; init; }
			
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Target target) => [];
				public static ValidationResult Validate(Target target, ValidationResult errors) => [];
			}
			"""
		).RunAsync();
}
