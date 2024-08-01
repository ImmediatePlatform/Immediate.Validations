using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Immediate.Validations.Shared.Localization;

internal sealed partial class ValidatorLocalizer : IStringLocalizer
{
	private static readonly Dictionary<string, Dictionary<string, LocalizedString>> s_localizations = new()
	{
		["en"] = En(),
		["fr"] = Fr(),
	};

	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
	{
		var currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
		return s_localizations[currentCulture]
			.Select(x => x.Value);
	}

	public LocalizedString this[string name]
	{
		get
		{
			var currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
			if (!s_localizations.ContainsKey(currentCulture))
				currentCulture = "en";

			if (s_localizations[currentCulture].TryGetValue(name, out var value))
				return value;

			return new LocalizedString(name, name, true);

		}
	}

	public LocalizedString this[string name, params object[] arguments] => this[name];
}
