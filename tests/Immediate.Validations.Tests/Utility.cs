using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace Immediate.Validations.Tests;

internal static class Utility
{
#if NET8_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net80;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net80.References.All;
#elif NET9_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net90;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net90.References.All;
#elif NET10_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net100;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net100.References.All;
#else
#error .net version not yet implemented
#endif

	public static IEnumerable<MetadataReference> GetMetadataReferences() =>
	[
		MetadataReference.CreateFromFile("./Immediate.Handlers.Shared.dll"),
		MetadataReference.CreateFromFile("./Immediate.Validations.Shared.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.Localization.Abstractions.dll"),
	];
}
