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
}
