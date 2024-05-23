namespace Immediate.Validations.Tests.GeneratorTests;

public sealed class CollectionTests
{
	[Fact]
	public async Task ListString()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				public List<string> StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ListListString()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				public List<List<string>> StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ListListStringNotEmpty()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using System.Collections.Generic;
			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[NotEmpty]
				public List<List<string>> StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ArrayString()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				public string[] StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ArrayArrayString()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				public string[][] StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ArrayArrayStringNotEmpty()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public partial class ValidateClass
			{
				[NotEmpty]
				public string[][] StringProperty { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ArrayValidationTarget()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable
			
			using Immediate.Validations.Shared;
			
			[Validate]
			public class ValidationTarget
			{
				public string StringProperty { get; init; }
			}
			
			[Validate]
			public partial class ValidateClass
			{
				public ValidationTarget[] ValidationTargets { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ArrayArrayValidationTarget()
	{
		var driver = GeneratorTestHelper.GetDriver(
			"""
			#nullable enable

			using Immediate.Validations.Shared;

			[Validate]
			public class ValidationTarget
			{
				public string StringProperty { get; init; }
			}

			[Validate]
			public partial class ValidateClass
			{
				public ValidationTarget[][] ValidationTargets { get; init; }
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result);
	}
}
