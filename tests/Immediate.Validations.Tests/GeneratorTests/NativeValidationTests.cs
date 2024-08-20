namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class NativeValidationTests
{
	[Fact]
	public async Task NotNullValidation()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
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

	[Fact]
	public async Task NullDoesNotUseNotNullValidation()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
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

	[Fact]
	public async Task EnumValidation()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			public enum TestEnum
			{
				None = 0,
				Valid,
				Invalid,
			}

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required TestEnum TestEnum { get; init; }
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
