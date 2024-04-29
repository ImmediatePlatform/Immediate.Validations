using Microsoft.CodeAnalysis;

namespace Immediate.Validations.Tests;

internal static class Utility
{
	public static MetadataReference[] GetMetadataReferences() =>
	[
		MetadataReference.CreateFromFile("./Immediate.Handlers.Shared.dll"),
		MetadataReference.CreateFromFile("./Immediate.Validations.Shared.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.Abstractions.dll"),
		MetadataReference.CreateFromFile("./Microsoft.AspNetCore.Authorization.dll"),
		MetadataReference.CreateFromFile("./Microsoft.AspNetCore.Metadata.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.Logging.Abstractions.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.Options.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.Primitives.dll"),
	];

	public static TheoryData<string> Methods() =>
		new([
			"Get",
			"Post",
			"Patch",
			"Put",
			"Delete",
		]);
}
