namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class MiscellaneousTests
{
	[Fact]
	public async Task PropertyValidationOnRecord()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.ComponentModel;
			using Immediate.Validations.Shared;

			[Validate]
			public sealed partial record ValidateClass(
				[property: MaxLength(3)] string Testing
			): IValidationTarget<ValidateClass>;
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

	[Fact]
	public async Task MultipleImplementedInterfacesBuildsCorrectly()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			[Validate]
			public partial interface IInterface1 : IValidationTarget<IInterface1>;
			[Validate]
			public partial interface IInterface2 : IValidationTarget<IInterface2>;
			
			public partial class Implementation : IInterface1, IInterface2;
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...IInterface1.g.cs",
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...IInterface2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task SkipSelfProperlySkipsSelf()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			[Validate]
			public partial interface IInterface1 : IValidationTarget<IInterface1>
			{
				[NotEmpty]
				string Property { get; }
			}
			
			[Validate(SkipSelf = true)]
			public partial class Implementation : IInterface1, IValidationTarget<Implementation>
			{
				public required string Property { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...IInterface1.g.cs",
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...Implementation.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}
}
