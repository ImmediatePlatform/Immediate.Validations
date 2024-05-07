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
}
