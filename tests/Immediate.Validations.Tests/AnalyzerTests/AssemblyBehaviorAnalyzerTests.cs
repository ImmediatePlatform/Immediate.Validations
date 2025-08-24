using Immediate.Validations.Analyzers;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class AssemblyBehaviorAnalyzerTests
{
	[Fact]
	public async Task BehaviorsAttributeHasValidationShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<AssemblyBehaviorAnalyzer>(
			"""
			using Immediate.Handlers.Shared;
			using Immediate.Validations.Shared;

			[assembly: Behaviors(
				typeof(ValidationBehavior<,>)
			)]
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task BehaviorsAttributeWithoutValidationShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<AssemblyBehaviorAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Handlers.Shared;

			[assembly: {|IV0011:Behaviors(
				typeof(DummyBehavior<,>)
			)|}]

			public sealed class DummyBehavior<TRequest, TResponse>
				: Behavior<TRequest, TResponse>
			{
				public override async ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
				{
					return await Next(request, cancellationToken).ConfigureAwait(false);
				}
			}
			
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task ClassBehaviorsAttributeWithoutValidationShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<AssemblyBehaviorAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Handlers.Shared;

			[Behaviors(
				typeof(DummyBehavior<,>)
			)]
			public static class Handler;

			public sealed class DummyBehavior<TRequest, TResponse>
				: Behavior<TRequest, TResponse>
			{
				public override async ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
				{
					return await Next(request, cancellationToken).ConfigureAwait(false);
				}
			}
			
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
