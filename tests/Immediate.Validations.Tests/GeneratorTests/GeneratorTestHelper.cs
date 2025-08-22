using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Immediate.Validations.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Validations.Tests.GeneratorTests;

public static class GeneratorTestHelper
{
	public static GeneratorDriverRunResult RunGenerator([StringSyntax("c#-test")] string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				.. Basic.Reference.Assemblies.Net80.References.All,
				.. Utility.GetMetadataReferences(),
			],
			options: new(
				outputKind: OutputKind.DynamicallyLinkedLibrary,
				specificDiagnosticOptions:
				[
					KeyValuePair.Create("CS0658", ReportDiagnostic.Suppress), // Suppress 'element' is not a recognized attribute location. Valid attribute locations for this declaration are 'field, property'. All attributes in this block will be ignored.
				]
			)
		);

		var clone = compilation.Clone().AddSyntaxTrees(CSharpSyntaxTree.ParseText("// dummy"));

		GeneratorDriver driver = CSharpGeneratorDriver.Create(
			generators: [new ImmediateValidationsGenerator().AsSourceGenerator()],
			driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true)
		);

		var result1 = RunGenerator(ref driver, compilation);
		var result2 = RunGenerator(ref driver, clone);

		foreach (var (_, step) in result2.Results[0].TrackedOutputSteps)
			AssertSteps(step);

		return result1;
	}

	private static GeneratorDriverRunResult RunGenerator(
		ref GeneratorDriver driver,
		Compilation compilation
	)
	{
		driver = driver
			.RunGeneratorsAndUpdateCompilation(
				compilation,
				out var outputCompilation,
				out var diagnostics
			);

		Assert.Empty(
			outputCompilation
				.GetDiagnostics()
				.Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
		);

		Assert.Empty(diagnostics);
		return driver.GetRunResult();
	}

	private static void AssertSteps(
		ImmutableArray<IncrementalGeneratorRunStep> steps
	)
	{
		var outputs = steps.SelectMany(o => o.Outputs);

		Assert.All(outputs, o => Assert.True(o.Reason is IncrementalStepRunReason.Unchanged or IncrementalStepRunReason.Cached));
	}
}
