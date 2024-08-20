namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class ClassLocationTests
{
	[Fact]
	public async Task NoNamespaceTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>;
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
	public async Task NamespaceTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			[Validate]
			public partial class ValidateClass : IValidationTarget<ValidateClass>;
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace..ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedClassesTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			public partial class OuterClass
			{
				[Validate]
				public partial class ValidateClass : IValidationTarget<ValidateClass>;
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace.OuterClass.ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedRecordsTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			public partial record OuterRecord
			{
				[Validate]
				public partial record ValidateRecord : IValidationTarget<ValidateRecord>;
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace.OuterRecord.ValidateRecord.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedRecordStructsTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			public partial record struct OuterRecordStruct
			{
				[Validate]
				public partial record struct ValidateRecordStruct : IValidationTarget<ValidateRecordStruct>;
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace.OuterRecordStruct.ValidateRecordStruct.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedInterfacesTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			public partial interface OuterInterface
			{
				[Validate]
				public partial interface ValidateClass : IValidationTarget<ValidateClass>;
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace.OuterInterface.ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedStructsTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			public partial struct OuterStruct
			{
				[Validate]
				public partial struct ValidateStruct : IValidationTarget<ValidateStruct>;
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace.OuterStruct.ValidateStruct.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task InheritBaseValidationTarget()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			[Validate]
			public partial class BaseClass : IValidationTarget<BaseClass>;

			[Validate]
			public partial class ValidateClass : BaseClass, IValidationTarget<ValidateClass>;
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace..BaseClass.g.cs",
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace..ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ImplementBaseValidationInterface()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			[Validate]
			public partial interface BaseInterface : IValidationTarget<BaseInterface>;

			[Validate]
			public partial class ValidateClass : BaseInterface, IValidationTarget<ValidateClass>;
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace..BaseInterface.g.cs",
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace..ValidateClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task InheritedClassesTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			public partial class OuterClass
			{
				[Validate]
				public partial class BaseClass : IValidationTarget<BaseClass>
				{
					public required int ValueA { get; init; }
				}

				[Validate]
				public partial class SubClass : BaseClass, IValidationTarget<SubClass>
				{
					[Equal(nameof(ValueA))]
					public required int ValueB { get; init; }
				}
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace.OuterClass.BaseClass.g.cs",
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace.OuterClass.SubClass.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task InheritedInterfacesTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Validations.Shared;

			namespace Namespace;

			[Validate]
			public partial interface IBaseInterface : IValidationTarget<IBaseInterface>
			{
				int ValueA { get; init; }
			}

			[Validate]
			public partial interface IInterface : IBaseInterface, IValidationTarget<IInterface>
			{
				[Equal(nameof(ValueA))]
				int ValueB { get; init; }
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace..IBaseInterface.g.cs",
				@"Immediate.Validations.Generators\Immediate.Validations.Generators.ImmediateValidationsGenerator\IV.Namespace..IInterface.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath)
		);

		_ = await Verify(result);
	}
}
