namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class CollectionTests
{
	[Fact]
	public async Task ListString()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required List<string> StringProperty { get; init; }
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
	public async Task ListListString()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required List<List<string>> StringProperty { get; init; }
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
	public async Task ListListStringNotEmpty()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEmpty]
				public required List<List<string>> StringProperty { get; init; }
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
	public async Task ArrayString()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required string[] StringProperty { get; init; }
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
	public async Task ArrayArrayString()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required string[][] StringProperty { get; init; }
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
	public async Task ArrayArrayStringNotEmpty()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEmpty]
				public required string[][] StringProperty { get; init; }
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
	public async Task ArrayValidationTarget()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable
			
			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidationTarget : IValidationTarget<ValidationTarget>
			{
				public required string StringProperty { get; init; }
			}
			
			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required ValidationTarget[] ValidationTargets { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidationTarget.g.cs",
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ArrayArrayValidationTarget()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidationTarget : IValidationTarget<ValidationTarget>
			{
				public required string StringProperty { get; init; }
			}

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				public required ValidationTarget[][] ValidationTargets { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidationTarget.g.cs",
				@"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}
}
