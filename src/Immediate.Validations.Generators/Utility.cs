using System.Reflection;
using System.Text.RegularExpressions;
using Scriban;

namespace Immediate.Validations.Generators;

internal static partial class Utility
{
	public static Template GetTemplate(string name)
	{
		using var stream = Assembly
			.GetExecutingAssembly()
			.GetManifestResourceStream(
				$"Immediate.Validations.Generators.Templates.{name}.sbntxt"
			)!;

		using var reader = new StreamReader(stream);
		return Template.Parse(reader.ReadToEnd());
	}

	private static readonly Regex s_toTitleCaseRegex =
		new(
			@"(?<=[^A-Z])([A-Z])",
			RegexOptions.Compiled,
			matchTimeout: TimeSpan.FromMilliseconds(10)
		);

	public static string ToTitleCase(this string pascalCase) =>
		s_toTitleCaseRegex
			.Replace(
				pascalCase,
				" $1"
			);

	public static string ToPascalCase(this string camelCase) =>
		$"{char.ToUpperInvariant(camelCase[0])}{camelCase[1..]}";
}
