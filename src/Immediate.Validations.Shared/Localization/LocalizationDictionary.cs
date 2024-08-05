using Microsoft.Extensions.Localization;

namespace Immediate.Validations.Shared.Localization;

internal sealed class LocalizationDictionary(Dictionary<string, string> localizations)
	: Dictionary<string, LocalizedString>(localizations.ToDictionary(x => x.Key, x => new LocalizedString(x.Key, x.Value)));
