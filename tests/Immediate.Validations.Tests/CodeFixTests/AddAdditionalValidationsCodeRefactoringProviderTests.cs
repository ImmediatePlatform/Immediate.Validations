using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class AddAdditionalValidationsCodeRefactoringProviderTests
{
	[Fact]
	public async Task RefactorOnValidatedClass() =>
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

	[Fact]
	public async Task NoRefactorOnNonValidatedClass() =>
		await CodeRefactoringTestHelper.CreateCodeRefactoringTest<AddAdditionalValidationsCodeRefactoringProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed record {|Refactoring:Data|}
			{
			}
			""",
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed record Data
			{
			}
			"""
		).RunAsync();
}
