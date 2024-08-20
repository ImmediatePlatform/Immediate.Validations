namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class ValidatorArgumentTests
{
	[Fact]
	public async Task ConstantAttributeArgument()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEqual(ArgumentValue)]
				public required string StringProperty { get; init; }

				private const string ArgumentValue = "Hello World";
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorSimple()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[Equal(0)]
				public required int IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorMessage()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[Equal(0, Message = "Must be equal to zero.")]
				public required int IntProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorNameof()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[Equal(nameof(KeyValue))]
				public required int IntProperty { get; init; }
				public required int KeyValue { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorNameofStatic()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[Equal(nameof(KeyValue))]
				public required int IntProperty { get; init; }
				public static int KeyValue { get; } = 123;
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorSimple()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[MaxLength(0)]
				public required string StringProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorMessage()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[MaxLength(0, Message = "Must be MaxLength to zero.")]
				public required string StringProperty { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorNameof()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[MaxLength(nameof(KeyValue))]
				public required string StringProperty { get; init; }
				public required int KeyValue { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorNameofStatic()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[MaxLength(nameof(KeyValue))]
				public required string StringProperty { get; init; }
				public static int KeyValue { get; } = 20;
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MethodAttributeArgument()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEqual(nameof(ArgumentValue))]
				public required string StringProperty { get; init; }

				private string ArgumentValue() => "Hello World";
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task StaticMethodAttributeArgument()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEqual(nameof(ArgumentValue))]
				public required string StringProperty { get; init; }

				private static string ArgumentValue() => "Hello World";
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task FieldAttributeArgument()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEqual(nameof(_argumentValue))]
				public required string StringProperty { get; init; }

				private readonly string _argumentValue = "Hello World";
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task StaticFieldAttributeArgument()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>
			{
				[NotEqual(nameof(_argumentValue))]
				public required string StringProperty { get; init; }

				private static readonly string _argumentValue = "Hello World";
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV...ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}
}
