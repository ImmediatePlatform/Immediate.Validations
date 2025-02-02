using Microsoft.Extensions.Localization;

namespace Immediate.Validations.Shared.Localization;

internal sealed class LocalizationDictionary(Dictionary<string, string> localizations)
	: Dictionary<string, LocalizedString>(
		localizations
			.Select(kvp => KeyValuePair.Create(kvp.Key, new LocalizedString(kvp.Key, kvp.Value))),
		StringComparer.Ordinal
	);
