using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class AddParameterToValidatePropertyMethodCodefixProviderTests
{
	[Fact]
	public async Task AddParameterFromProperty() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, AddParameterToValidatePropertyMethodCodefixProvider>(
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int {|IV0005:Operand|} { get; init; }
			
				public static bool ValidateProperty(int value) =>
					value <= 1;
			
				public const string DefaultMessage = "";
			}
			""",
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }
			
				public static bool ValidateProperty(int value, int operand) =>
					value <= 1;
			
				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Fact]
	public async Task AddParameterFromPrimaryConstructorParameter() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, AddParameterToValidatePropertyMethodCodefixProvider>(
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute(int {|IV0005:operand|}) : ValidatorAttribute
			{
				public static bool ValidateProperty(int value) =>
					value <= 1;
			
				public const string DefaultMessage = "";
			}
			""",
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute(int {|IV0005:operand|}) : ValidatorAttribute
			{
				public static bool ValidateProperty(int value, int operand) =>
					value <= 1;
			
				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

	[Fact]
	public async Task AddParameterFromConstructorParameter() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, AddParameterToValidatePropertyMethodCodefixProvider>(
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				private readonly int _operand;
			
				public GreaterThanAttribute(int {|IV0005:operand|})
				{
					_operand = operand;
				}
			
				public static bool ValidateProperty(int value) =>
					value <= 1;
			
				public const string DefaultMessage = "";
			}
			""",
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				private readonly int _operand;
			
				public GreaterThanAttribute(int operand)
				{
					_operand = operand;
				}
			
				public static bool ValidateProperty(int value, int operand) =>
					value <= 1;
			
				public const string DefaultMessage = "";
			}
			"""
		).RunAsync();

}
