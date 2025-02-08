using Microsoft.CodeAnalysis;

namespace Immediate.Validations.Generators;

internal static class DisplayNameFormatters
{
	public static readonly SymbolDisplayFormat FullyQualifiedForMembers =
		SymbolDisplayFormat.FullyQualifiedFormat
			.WithMemberOptions(
				SymbolDisplayMemberOptions.IncludeContainingType
				| SymbolDisplayMemberOptions.IncludeParameters
			);
}
