using Microsoft.CodeAnalysis;

namespace Immediate.Validations.Tests;

internal static class Utility
{
	public static MetadataReference[] GetMetadataReferences() =>
	[
		MetadataReference.CreateFromFile("./Immediate.Handlers.Shared.dll"),
		MetadataReference.CreateFromFile("./Immediate.Validations.Shared.dll"),
	];
}
