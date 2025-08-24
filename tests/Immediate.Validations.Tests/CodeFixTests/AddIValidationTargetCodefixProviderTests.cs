using Immediate.Validations.Analyzers;
using Immediate.Validations.CodeFixes;

namespace Immediate.Validations.Tests.CodeFixTests;

public sealed class AddIValidationTargetCodefixProviderTests
{
	[Fact]
	public async Task AddIValidationTargetNoBaseTypes() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidateClassAnalyzer, AddIValidationTargetCodefixProvider>(
			"""
			namespace Immediate.Validations.Shared;

			[Validate]
			public sealed record {|IV0013:Data|}
			{
			}
			""",
			"""
			namespace Immediate.Validations.Shared;

			[Validate]
			public sealed record Data : {|CS0535:{|CS0535:IValidationTarget<Data>|}|}
			{
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AddIValidationTargetWithBaseType() =>
		await CodeFixTestHelper.CreateCodeFixTest<ValidateClassAnalyzer, AddIValidationTargetCodefixProvider>(
			"""
			using System;

			namespace Immediate.Validations.Shared;

			[Validate]
			public sealed record {|IV0013:Data|} : IDisposable
			{
				public void Dispose() { }
			}
			""",
			"""
			using System;

			namespace Immediate.Validations.Shared;

			[Validate]
			public sealed record Data : IDisposable, {|CS0535:{|CS0535:IValidationTarget<Data>|}|}
			{
				public void Dispose() { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
