using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class CorrectValidatePropertyReturnTypeCodefixProviderTests
{
	[Fact]
	public async Task CorrectValidatePropertyReturnType() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, CorrectValidatePropertyReturnTypeCodefixProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed class TestAttribute : ValidatorAttribute
			{
				public static string {|IV0004:ValidateProperty|}<T>(T value) => default;
			
				public const string DefaultMessage = "";
			}
			""",
			$$"""
			namespace Immediate.Validations.Shared;

			public sealed class TestAttribute : ValidatorAttribute
			{
				public static bool ValidateProperty<T>(T value) => default;
			
				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();
}
