namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class MiscellaneousTests
{
	[Fact]
	public async Task FilledDescriptionChangesName()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.ComponentModel;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[Description("Hello World!")]
				public required string Testing { get; init; }
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

	[Fact]
	public async Task EmptyDescriptionDoesntChangeName()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.ComponentModel;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[Description]
				public required string Testing { get; init; }
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
