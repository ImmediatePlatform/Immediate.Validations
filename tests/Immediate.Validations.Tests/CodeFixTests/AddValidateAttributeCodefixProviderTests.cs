using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class AddValidateAttributeCodefixProviderTests
{
	[Fact]
	public async Task AddValidateAttribute() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidateClassAnalyzer, AddValidateAttributeCodefixProvider>(
			"""
			namespace Immediate.Validations.Testing;
			
			public sealed record {|IV0012:Data|} : Immediate.Validations.Shared.IValidationTarget<Data>
			{
				public Immediate.Validations.Shared.ValidationResult Validate() => [];
				public Immediate.Validations.Shared.ValidationResult Validate(Immediate.Validations.Shared.ValidationResult errors) => [];
				public static Immediate.Validations.Shared.ValidationResult Validate(Data target) => [];
				public static Immediate.Validations.Shared.ValidationResult Validate(Data target, Immediate.Validations.Shared.ValidationResult errors) => [];
			}
			""",
			"""
			using Immediate.Validations.Shared;

			namespace Immediate.Validations.Testing;
			
			[Validate]
			public sealed record Data : Immediate.Validations.Shared.IValidationTarget<Data>
			{
				public Immediate.Validations.Shared.ValidationResult Validate() => [];
				public Immediate.Validations.Shared.ValidationResult Validate(Immediate.Validations.Shared.ValidationResult errors) => [];
				public static Immediate.Validations.Shared.ValidationResult Validate(Data target) => [];
				public static Immediate.Validations.Shared.ValidationResult Validate(Data target, Immediate.Validations.Shared.ValidationResult errors) => [];
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AddValidateAttributeWorksCorrectlyWithOtherAttributes() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidateClassAnalyzer, AddValidateAttributeCodefixProvider>(
			"""
			using System;

			namespace Immediate.Validations.Shared;

			[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
			public sealed class MyStuffAttribute : Attribute;
			
			[MyStuff]
			public sealed record {|IV0012:Data|} : IValidationTarget<Data>
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Data target) => [];
				public static ValidationResult Validate(Data target, ValidationResult errors) => [];
			}
			""",
			"""
			using System;
			
			namespace Immediate.Validations.Shared;
			
			[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
			public sealed class MyStuffAttribute : Attribute;
			
			[MyStuff]
			[Validate]
			public sealed record {|IV0012:Data|} : IValidationTarget<Data>
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Data target) => [];
				public static ValidationResult Validate(Data target, ValidationResult errors) => [];
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AddValidateAttributeWorksCorrectlyWithXmlDocs() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidateClassAnalyzer, AddValidateAttributeCodefixProvider>(
			"""
			namespace Immediate.Validations.Shared;
			
			/// <summary>documentation</summary>
			public sealed record {|IV0012:Data|} : IValidationTarget<Data>
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Data target) => [];
				public static ValidationResult Validate(Data target, ValidationResult errors) => [];
			}
			""",
			"""
			namespace Immediate.Validations.Shared;
			
			/// <summary>documentation</summary>
			[Validate]
			public sealed record Data : IValidationTarget<Data>
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Data target) => [];
				public static ValidationResult Validate(Data target, ValidationResult errors) => [];
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AddValidateAttributeWorksCorrectlyWithXmlDocsAndOtherAttributes() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidateClassAnalyzer, AddValidateAttributeCodefixProvider>(
			"""
			using System;

			namespace Immediate.Validations.Shared;

			[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
			public sealed class MyStuffAttribute : Attribute;
			
			/// <summary>documentation</summary>
			[MyStuff]
			public sealed record {|IV0012:Data|} : IValidationTarget<Data>
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Data target) => [];
				public static ValidationResult Validate(Data target, ValidationResult errors) => [];
			}
			""",
			"""
			using System;
			
			namespace Immediate.Validations.Shared;
			
			[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
			public sealed class MyStuffAttribute : Attribute;
			
			/// <summary>documentation</summary>
			[MyStuff]
			[Validate]
			public sealed record {|IV0012:Data|} : IValidationTarget<Data>
			{
				public ValidationResult Validate() => [];
				public ValidationResult Validate(ValidationResult errors) => [];
				public static ValidationResult Validate(Data target) => [];
				public static ValidationResult Validate(Data target, ValidationResult errors) => [];
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
