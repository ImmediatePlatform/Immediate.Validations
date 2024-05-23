using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class ValidatorClassCodefixTests
{
	[Fact]
	public async Task AddValidateMethod()
	{
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, AddValidateMethodCodefixProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed class {|IV0001:TestAttribute|} : ValidatorAttribute
			{
			}
			""",
			$$"""
			namespace Immediate.Validations.Shared;

			public sealed class TestAttribute : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty<T>(T value) => default;
			}
			"""
		).RunAsync();
	}

	[Fact]
	public async Task MakeValidatePropertyMethodStatic()
	{
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, MakeValidatePropertyMethodStaticCodefixProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed class TestAttribute : ValidatorAttribute
			{
				public (bool Invalid, string? DefaultMessage) {|IV0002:ValidateProperty|}<T>(T value) => default;
			}
			""",
			$$"""
			namespace Immediate.Validations.Shared;

			public sealed class TestAttribute : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty<T>(T value) => default;
			}
			"""
		).RunAsync();
	}
}
