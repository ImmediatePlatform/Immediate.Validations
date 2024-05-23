using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class ValidatorClassCodefixTests
{
	[Fact]
	public async Task MissingValidatePropertyMethodShouldAddMethod()
	{
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, ValidateMethodMustExistCodefixProvider>(
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
}
