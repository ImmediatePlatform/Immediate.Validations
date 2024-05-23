using System.Diagnostics.CodeAnalysis;
using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test methods")]
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

	[Fact]
	public async Task CorrectValidatePropertyReturnType()
	{
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, CorrectValidatePropertyReturnTypeCodefixProvider>(
			$$"""
			namespace Immediate.Validations.Shared;
			
			public sealed class TestAttribute : ValidatorAttribute
			{
				public static string {|IV0004:ValidateProperty|}<T>(T value) => default;
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
	public async Task AddParameterToValidatePropertyMethod_FromProperty()
	{
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, AddParameterToValidatePropertyMethodCodefixProvider>(
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int {|IV0005:Operand|} { get; init; }
			
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value)
				{
					return value <= -1
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			""",
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute : ValidatorAttribute
			{
				public required int Operand { get; init; }
			
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value, int operand)
				{
					return value <= -1
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();
	}

	[Fact]
	public async Task AddParameterToValidatePropertyMethod_FromPrimaryConstructorParameter()
	{
		await CodeFixTestHelper.CreateCodeFixTest<ValidatorClassAnalyzer, AddParameterToValidatePropertyMethodCodefixProvider>(
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute(int {|IV0005:operand|}) : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value)
				{
					return value <= -1
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			""",
			$$"""
			using Immediate.Validations.Shared;
			
			public sealed class GreaterThanAttribute(int {|IV0005:operand|}) : ValidatorAttribute
			{
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value, int operand)
				{
					return value <= -1
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();
	}

	[Fact]
	public async Task AddParameterToValidatePropertyMethod_FromConstructorParameter()
	{
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
			
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value)
				{
					return value <= -1
						? (true, "Property must not be `null`.")
						: default;
				}
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
			
				public static (bool Invalid, string? DefaultMessage) ValidateProperty(int value, int operand)
				{
					return value <= -1
						? (true, "Property must not be `null`.")
						: default;
				}
			}
			"""
		).RunAsync();
	}
}
