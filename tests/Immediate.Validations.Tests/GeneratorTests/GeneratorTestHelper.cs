using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Immediate.Validations.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Validations.Tests.GeneratorTests;

public static class GeneratorTestHelper
{
	public static GeneratorDriverRunResult RunGenerator(
		[StringSyntax("c#-test")] string source,
		params ReadOnlySpan<string> skippedSteps
	) => RunGenerator(source, [], includeOpenApi: false, skippedSteps);

	public static GeneratorDriverRunResult RunOpenApiGenerators(
		[StringSyntax("c#-test")] string source,
		params ReadOnlySpan<MetadataReference> additionalReferences
	) => RunGenerator(source, additionalReferences, includeOpenApi: true, []);

	private static GeneratorDriverRunResult RunGenerator(
		string source,
		ReadOnlySpan<MetadataReference> additionalReferences,
		bool includeOpenApi,
		ReadOnlySpan<string> skippedSteps
	)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(
			source,
			cancellationToken: TestContext.Current.CancellationToken
		);

		// The validation generator supports the custom `element` attribute target.
		var diagnosticOptions = ImmutableDictionary<string, ReportDiagnostic>.Empty.Add("CS0658", ReportDiagnostic.Suppress);
		if (includeOpenApi)
		{
			// Match the SDK's assembly version unification for references like Microsoft.OpenApi targeting .NET 8.
			diagnosticOptions = diagnosticOptions.Add("CS1701", ReportDiagnostic.Suppress);
		}

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				..Utility.NetCoreAssemblies,
				..Utility.GetMetadataReferences(),
				..additionalReferences,
			],
			options: new(
				outputKind: OutputKind.DynamicallyLinkedLibrary,
				specificDiagnosticOptions: diagnosticOptions
			)
		);

		GeneratorDriver driver = CSharpGeneratorDriver.Create(
			generators: includeOpenApi
				? [new ImmediateValidationsGenerator().AsSourceGenerator(), new OpenApiIntegrationGenerator().AsSourceGenerator()]
				: [new ImmediateValidationsGenerator().AsSourceGenerator()],
			driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true)
		);

		driver = RunGenerator(driver, compilation);
		var result = driver.GetRunResult();

		VerifyIncrementality(driver, compilation, skippedSteps);

		return result;
	}

	private static GeneratorDriver RunGenerator(
		GeneratorDriver driver,
		Compilation compilation
	)
	{
		driver = driver
			.RunGeneratorsAndUpdateCompilation(
				compilation,
				out var outputCompilation,
				out var diagnostics,
				TestContext.Current.CancellationToken
			);

		Assert.Empty(
			outputCompilation
				.GetDiagnostics(TestContext.Current.CancellationToken)
				.Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
		);

		Assert.Empty(diagnostics);
		return driver;
	}

	private static void VerifyIncrementality(
		GeneratorDriver driver,
		Compilation compilation,
		ReadOnlySpan<string> skippedSteps
	)
	{
		var clone = compilation.Clone().AddSyntaxTrees(
			CSharpSyntaxTree.ParseText(
				"// dummy",
				cancellationToken: TestContext.Current.CancellationToken
			)
		);

		driver = RunGenerator(driver, clone);

		var results = driver.GetRunResult().Results;
		foreach (var (_, step) in results.SelectMany(r => r.TrackedOutputSteps))
			AssertSteps(step);

		var trackedSteps = results[0].TrackedSteps;
		foreach (var step in TrackedSteps)
		{
			if (skippedSteps.Contains(step))
			{
				if (trackedSteps.ContainsKey(step))
					Assert.Fail($"Step `{step}` should have been skipped, but is present.");
			}
			else
			{
				if (!trackedSteps.TryGetValue(step, out var outputs))
					Assert.Fail($"Step `{step}` expected, but is missing.");

				AssertSteps(outputs);
			}
		}
	}

	private static ReadOnlySpan<string> TrackedSteps =>
		new string[]
		{
			"ValidationClasses",
		};

	private static void AssertSteps(
		ImmutableArray<IncrementalGeneratorRunStep> steps
	)
	{
		var outputs = steps.SelectMany(o => o.Outputs);

		Assert.All(outputs, o => Assert.True(o.Reason is IncrementalStepRunReason.Unchanged or IncrementalStepRunReason.Cached));
	}
}
