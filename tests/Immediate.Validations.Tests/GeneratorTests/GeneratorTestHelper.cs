using Immediate.Validations.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Validations.Tests.GeneratorTests;

public static class GeneratorTestHelper
{
	public static GeneratorDriver GetDriver(string source)
	{
		// Parse the provided string into a C# syntax tree
		var syntaxTree = CSharpSyntaxTree.ParseText(source);

		// Create a Roslyn compilation for the syntax tree.
		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				.. Basic.Reference.Assemblies.Net80.References.All,
				.. Utility.GetMetadataReferences(),
			]
		);

		// Create an instance of our incremental source generator
		var generator = new ImmediateValidationsGenerator();

		// The GeneratorDriver is used to run our generator against a compilation
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

		// Run the source generator!
		return driver.RunGenerators(compilation);
	}
}
