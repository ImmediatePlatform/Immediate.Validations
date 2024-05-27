using Immediate.Validations.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeQuality.Analyzers.ApiDesignGuidelines;

namespace Immediate.Validations.Tests.AnalyzerTests;

public sealed class UnusedConstructorParameterSuppressorTests
{
	public static readonly DiagnosticResult CS9113 =
		DiagnosticResult.CompilerError("CS9113");

	public static readonly DiagnosticResult CA1019 =
		DiagnosticResult.CompilerWarning("CA1019");

	[Fact]
	public async Task WarningsInValidatorAreSuppressed() =>
		await AnalyzerTestHelpers
			.CreateSuppressorTest<UnusedConstructorParameterSuppressor>(
				"""
				#nullable enable

				using Immediate.Validations.Shared;
			
				public sealed class GreaterThanAttribute(int {|#0:operand|}) : ValidatorAttribute
				{
				}
				"""
			)
			.WithAnalyzer<DefineAccessorsForAttributeArgumentsAnalyzer>()
			.WithSpecificDiagnostics([CS9113, CA1019])
			.WithExpectedDiagnosticsResults([
				CS9113.WithLocation(0).WithIsSuppressed(true),
				CA1019.WithLocation(0).WithIsSuppressed(true),
			])
			.RunAsync();

	[Fact]
	public async Task WarningsAllowedElsewhere() =>
		await AnalyzerTestHelpers
			.CreateSuppressorTest<UnusedConstructorParameterSuppressor>(
				"""
				#nullable enable

				using System;
			
				public sealed class GreaterThanAttribute(int {|#0:operand|}) : Attribute
				{
				}
				"""
			)
			.WithAnalyzer<DefineAccessorsForAttributeArgumentsAnalyzer>()
			.WithSpecificDiagnostics([CS9113, CA1019])
			.WithExpectedDiagnosticsResults([
				CS9113.WithLocation(0).WithIsSuppressed(false),
				CA1019.WithLocation(0).WithIsSuppressed(false),
			])
			.RunAsync();
}
