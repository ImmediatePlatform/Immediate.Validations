using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class AddAdditionalValidationsCodeRefactoringProviderTests
{
	[Fact]
	public async Task AddValidateMethod() =>
		await CodeRefactoringTestHelper.CreateCodeRefactoringTest<AddAdditionalValidationsCodeRefactoringProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			[Validate]
			public sealed record {|Refactoring:Data|}
			{
			}
			""",
			$$"""
			namespace Immediate.Validations.Shared;
			
			[Validate]
			public sealed record Data
			{
				private static void AdditionalValidations(ValidationResult errors, Data target)
				{
				}
			}
			"""
		).RunAsync();
}
