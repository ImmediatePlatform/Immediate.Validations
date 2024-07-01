using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class AddValidateMethodCodefixProviderTests
{
	[Fact]
	public async Task AddValidateMethod() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, AddValidateMethodCodefixProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed class {|IV0001:TestAttribute|} : ValidatorAttribute
			{
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
