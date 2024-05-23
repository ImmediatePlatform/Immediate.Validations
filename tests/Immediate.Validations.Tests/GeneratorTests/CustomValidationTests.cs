namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class CustomValidationTests
{
	[Fact]
	public async Task NotEmptyOrWhiteSpaceOnString()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[NotEmptyOrWhiteSpace]
				public string StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NotEmptyOrWhiteSpaceOnInt()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[NotEmptyOrWhiteSpace]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task CustomValidationOnProperType()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public class IntGreaterThanZeroAttribute : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value) =>
					value <= 0
						? (true, "Property must not be `null` or whitespace.")
						: default;
			}

			[Validate]
			public partial class ValidateClass
			{
				[IntGreaterThanZero]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task CustomValidationOnInvalidType()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public class IntGreaterThanZeroAttribute : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value) =>
					value <= 0
						? (true, "Property must not be `null` or whitespace.")
						: default;
			}

			[Validate]
			public partial class ValidateClass
			{
				[IntGreaterThanZero]
				public string StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task CustomValidationMissingValidateMethod()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public class IntGreaterThanZeroAttribute : ValidatorAttribute
			{
			}

			[Validate]
			public partial class ValidateClass
			{
				[IntGreaterThanZero]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NotNullAsCustomValidationOnGenericType()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[NotNull]
				public string? StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NotNullAsCustomValidationOnInvalidGenericType()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[NotNull]
				public int? IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EnumAsCustomValidationOnGenericType()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public enum TestEnum { None = 0, Valid = 1 }

			[Validate]
			public partial class ValidateClass
			{
				public TestEnum? EnumProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task CustomValidatorWithParameters()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }

				public static (bool Invalid, string Message) ValidateProperty(int value, int operand) =>
					value > operand ? default : (true, $"Value `{value}` is not greater than `{operand}`.");
			}

			[Validate]
			public partial class ValidateClass
			{
				[GreaterThan(Operand = 0, Message = "Must be greater than zero.")]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorSimple()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Equal(0)]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorMessage()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Equal(0, Message = "Must be equal to zero.")]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorNameof()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Equal(nameof(KeyValue))]
				public int IntProperty { get; init; }
				public int KeyValue { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorSimple()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[MaxLength(0)]
				public string StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorMessage()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[MaxLength(0, Message = "Must be MaxLength to zero.")]
				public string StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorNameof()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[MaxLength(nameof(KeyValue))]
				public string StringProperty { get; init; }
				public int KeyValue { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ComplexValidator()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] object first,
				string second
			) : ValidatorAttribute
			{
				public required string Third { get; init; }

				public static (bool Invalid, string? Message) ValidateProperty(
					string target,
					string first,
					string second,
					string third,
					string fourth = "fourth"
				) =>
					target == $"{first}-{second}-{third}-{fourth}"
						? default
						: (true, $"Value '{target}' is not equal to '{first}-{second}-{third}-{fourth}'");
			}
						
			[Validate]
			public sealed partial record Target : IValidationTarget<Target>
			{
				[Dummy(first: nameof(FirstValue), "Hello World", Third = "Value", Message = "What's going on?")]
				public required string Id { get; init; }
				public required string FirstValue { get; init; }

				public static List<ValidationError> Validate(Target target) => [];
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ParamsConstructor()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			public sealed class DummyAttribute(
				[TargetType] object first,
				string second,
				[TargetType] params object[] third
			) : ValidatorAttribute
			{
				public required string Fourth { get; init; }
				public required string Fifth { get; init; }

				public static (bool Invalid, string? Message) ValidateProperty(
					string target,
					string first,
					string second,
					string fourth,
					string fifth,
					params string[] third
				) =>
					target == first
						? default
						: (true, $"Value '{target}' is not equal to '{first}'");
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

				public static List<ValidationError> Validate(Target target) => [];
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task AdditionalValidations()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				private static IEnumerable<ValidationError> AdditionalValidations(ValidateClass target) => [];
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task AdditionalValidationsInherited()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class BaseClass : IValidationTarget<BaseClass>
			{
				private static IEnumerable<ValidationError> AdditionalValidations(BaseClass target) => [];
			}
			
			[Validate]
			public partial class SubClass : BaseClass, IValidationTarget<SubClass>;
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result);
	}
}
