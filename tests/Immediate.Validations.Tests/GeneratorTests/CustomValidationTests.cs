namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class CustomValidationTests
{
	[Test]
	public async Task NotEmptyOnString()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEmpty]
				public required string StringProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task NotEmptyOnInt()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEmpty]
				public required int IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task NotNullOnInt()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotNull]
				public required int IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task NotNullOnNullableInt()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotNull]
				public required int? IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task NotNullOnString()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotNull]
				public required string? StringProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task CustomValidationOnProperType()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public class IntGreaterThanZeroAttribute : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value) =>
					value <= 0
						? (true, "Property must not be empty.")
						: default;
			}

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[IntGreaterThanZero]
				public required int IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task CustomValidationOnInvalidType()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public class IntGreaterThanZeroAttribute : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value) =>
					value <= 0
						? (true, "Property must not be empty.")
						: default;
			}

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[IntGreaterThanZero]
				public required string StringProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task CustomValidationMissingValidateMethod()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public class IntGreaterThanZeroAttribute : ValidatorAttribute
			{
			}

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[IntGreaterThanZero]
				public required int IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task NotNullAsCustomValidationOnGenericType()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotNull]
				public required string? StringProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task NotNullAsCustomValidationOnInvalidGenericType()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public sealed class NotNullClassAttribute : ValidatorAttribute
			{
				public static bool ValidateProperty<T>(T value)
					where T : class =>
					value is not null;

				public static string DefaultMessage => ValidationConfiguration.Localizer[nameof(NotNullAttribute)].Value;
			}
			
			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotNullClass]
				public required int? IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task EnumAsCustomValidationOnGenericType()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public enum TestEnum { None = 0, Valid = 1 }

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required TestEnum? EnumProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task CustomValidatorWithParameters()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public static bool ValidateProperty(int value, int operand) =>
					value > operand;
			}

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[GreaterThan(Operand = 0, Message = "Must be greater than zero.")]
				public int IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task ComplexValidator()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			#pragma warning disable CS9113

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
				) => target == $"{first}-{second}-{third}-{fourth}";
			}
						
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy(first: nameof(FirstValue), "Hello World", Third = "Value", Message = "What's going on?")]
				public required string Id { get; init; }
				public required string FirstValue { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...Target.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task ParamsConstructor()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			#pragma warning disable CS9113

			public sealed class DummyAttribute(
				[TargetType] object first,
				string second,
				[TargetType] params object[] third
			) : ValidatorAttribute
			{
				public required string Fourth { get; init; }
				public required string Fifth { get; init; }

				public static bool ValidateProperty(
					string target,
					string first,
					string second,
					string fourth,
					string fifth,
					params string[] third
				) => target == first;
			}
						
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy(
					first: nameof(FirstValue),
					"Hello World",
					"Test1",
					nameof(FirstValue),
					"Test3",
					Fourth = "Abcd",
					Message = "What's going on?",
					Fifth = "The end?"
				)]
				public required string Id { get; init; }
				public required string FirstValue { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...Target.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task AdditionalValidations()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				private static void AdditionalValidations(List<ValidationError> errors, ValidateClass target) { }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task AdditionalValidationsInherited()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class BaseClass : IValidationTarget<BaseClass>
			{
				private static void AdditionalValidations(List<ValidationError> errors, BaseClass target) { }
			}
			
			[Validate]
			public partial class SubClass : BaseClass, IValidationTarget<SubClass>;
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...BaseClass.g.cs",
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...SubClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task OneOfWithArrayField()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[OneOf(nameof(s_validStrings))]
				public required string StringProperty { get; init; }

				private static readonly string[] s_validStrings = ["123"];
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Test]
	public async Task OneOfWithArrayValue()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[OneOf(new object[] { "123" })]
				public required string StringProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}
}
