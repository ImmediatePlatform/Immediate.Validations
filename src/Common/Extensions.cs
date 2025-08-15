using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Validations;

[ExcludeFromCodeCoverage]
internal static class Extensions
{
	public static string? NullIf(this string value, string check) =>
		value.Equals(check, StringComparison.Ordinal) ? null : value;

	public static T? SingleValue<T>(this IEnumerable<T> source)
	{
		using var enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
			return default;

		var c = enumerator.Current;
		if (enumerator.MoveNext())
			return default;

		return c;
	}

	public static bool IsValidConversion(this Conversion conversion) =>
		conversion is { IsIdentity: true }
			or { IsImplicit: true, IsReference: true }
			or { IsImplicit: true, IsNullable: true }
			or { IsBoxing: true };
}
