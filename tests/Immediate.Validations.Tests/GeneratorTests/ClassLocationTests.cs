namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class ClassLocationTests
{
	[Fact]
	public async Task NoNamespaceTest()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass;
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NamespaceTest()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			[Validate]
			public partial class ValidateClass;
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedClassesTest()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			public partial class OuterClass
			{
				[Validate]
				public partial class ValidateClass;
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedRecordsTest()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			public partial record OuterRecord
			{
				[Validate]
				public partial record ValidateRecord;
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedRecordStructsTest()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			public partial record struct OuterRecordStruct
			{
				[Validate]
				public partial record struct ValidateRecordStruct;
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedInterfacesTest()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			public partial interface OuterInterface
			{
				[Validate]
				public partial interface ValidateClass;
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task NestedStructsTest()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			public partial struct OuterStruct
			{
				[Validate]
				public partial struct ValidateStruct;
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task InheritBaseValidationTarget()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			[Validate]
			public partial class BaseClass : IValidationTarget<BaseClass>;

			[Validate]
			public partial class ValidateClass : BaseClass, IValidationTarget<ValidateClass>;
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ImplementBaseValidationInterface()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			using Immediate.Validations.Shared;

			using Namespace

			[Validate]
			public partial interface BaseInterface : IValidationTarget<BaseInterface>;

			[Validate]
			public partial class ValidateClass : BaseInterface, IValidationTarget<ValidateClass>;
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result);
	}
}
