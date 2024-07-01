using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class AddValidateAttributeCodefixProviderTests
{
	[Fact]
	public async Task AddValidateAttribute() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidateClassAnalyzer, AddValidateAttributeCodefixProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed record {|IV0012:Data|} : IValidationTarget<Data>
			{
				public static ValidationResult Validate(Data target) => [];
			}
			""",
			$$"""
			namespace Immediate.Validations.Shared;
			
			[Validate]
			public sealed record Data : IValidationTarget<Data>
			{
				public static ValidationResult Validate(Data target) => [];
			}
			"""
		).RunAsync();
}
