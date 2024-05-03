namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class NativeValidationTests
{
	[Fact]
	public async Task NotNullValidation()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				public string StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NullDoesNotUseNotNullValidation()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				public string? StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EnumValidation()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			public enum TestEnum
			{
				None = 0,
				Valid,
				Invalid,
			}

			[Validate]
			public partial class ValidateClass
			{
				public TestEnum TestEnum { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}
}
