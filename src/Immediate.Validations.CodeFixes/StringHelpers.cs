namespace Immediate.Validations.CodeFixes;

public static class StringHelpers
{
	public static string ToCamelCase(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}

		return char.ToLowerInvariant(value[0]) + value[1..];
	}
}
