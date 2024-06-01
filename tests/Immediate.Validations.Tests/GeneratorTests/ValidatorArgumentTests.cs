namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class ValidatorArgumentTests
{
	[Fact]
	public async Task ConstantAttributeArgument()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass
			{
				[NotEqual(ArgumentValue)]
				public string StringProperty { get; init; }

				private const string ArgumentValue = "Hello World";
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorSimple()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Equal(0)]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorMessage()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Equal(0, Message = "Must be equal to zero.")]
				public int IntProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorNameof()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Equal(nameof(KeyValue))]
				public int IntProperty { get; init; }
				public int KeyValue { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task EqualValidatorNameofStatic()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[Equal(nameof(KeyValue))]
				public int IntProperty { get; init; }
				public static int KeyValue { get; } = 123;
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorSimple()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[MaxLength(0)]
				public string StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorMessage()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[MaxLength(0, Message = "Must be MaxLength to zero.")]
				public string StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorNameof()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[MaxLength(nameof(KeyValue))]
				public string StringProperty { get; init; }
				public int KeyValue { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MaxLengthValidatorNameofStatic()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[MaxLength(nameof(KeyValue))]
				public string StringProperty { get; init; }
				public static int KeyValue { get; } = 20;
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task MethodAttributeArgument()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass
			{
				[NotEqual(nameof(ArgumentValue))]
				public string StringProperty { get; init; }

				private string ArgumentValue() => "Hello World";
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task StaticMethodAttributeArgument()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass
			{
				[NotEqual(nameof(ArgumentValue))]
				public string StringProperty { get; init; }

				private static string ArgumentValue() => "Hello World";
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task FieldAttributeArgument()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass
			{
				[NotEqual(nameof(_argumentValue))]
				public string StringProperty { get; init; }

				private readonly string _argumentValue = "Hello World";
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task StaticFieldAttributeArgument()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;
			
			[Validate]
			public partial class ValidateClass
			{
				[NotEqual(nameof(_argumentValue))]
				public string StringProperty { get; init; }

				private static readonly string _argumentValue = "Hello World";
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}
}
