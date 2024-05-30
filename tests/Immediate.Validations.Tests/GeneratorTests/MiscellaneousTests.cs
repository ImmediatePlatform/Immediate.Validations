namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class MiscellaneousTests
{
	[Fact]
	public async Task FilledDescriptionChangesName()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using System.ComponentModel;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Description("Hello World!")]
				public required string Testing { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EmptyDescriptionDoesntChangeName()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using System.ComponentModel;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Description]
				public required string Testing { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}
}
