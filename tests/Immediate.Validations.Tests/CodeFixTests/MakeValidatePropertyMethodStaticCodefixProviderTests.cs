using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class MakeValidatePropertyMethodStaticCodefixProviderTests
{
	[Fact]
	public async Task MakeValidatePropertyMethodStatic() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, MakeValidatePropertyMethodStaticCodefixProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed class TestAttribute : ValidatorAttribute
			{
				public bool {|IV0002:ValidateProperty|}<T>(T value) => default;
			
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
