using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Immediate.Validations.Shared;

internal sealed class ValidatorLocalizer : IStringLocalizer
{
	private readonly Dictionary<string, Dictionary<string, LocalizedString>> _localizations = GetLocalizations();

	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
	{
		var currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
		return _localizations[currentCulture].Select(x => x.Value);
	}

	public LocalizedString this[string name]
	{
		get
		{
			var currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
			if (!_localizations.ContainsKey(currentCulture))
				currentCulture = "en";

			if (_localizations[currentCulture].TryGetValue(name, out var value))
				return value;

			return new LocalizedString(name, name, true);

		}
	}

	public LocalizedString this[string name, params object[] arguments] => this[name];

	private static Dictionary<string, Dictionary<string, LocalizedString>> GetLocalizations()
	{
		return new Dictionary<string, Dictionary<string, string>>
		{
			{
				"en",
				new Dictionary<string, string>
				{
					{ nameof(EmptyAttribute), "'{PropertyName}' must be empty." },
					{ nameof(EnumValueAttribute), "'{PropertyName}' has a range of values which does not include '{PropertyValue}'." },
					{ nameof(EqualAttribute), "'{PropertyName}' must be equal to '{ComparisonValue}'." },
					{ nameof(GreaterThanAttribute), "'{PropertyName}' must be greater than '{ComparisonValue}'." },
					{ nameof(GreaterThanOrEqualAttribute), "'{PropertyName}' must be greater than or equal to '{ComparisonValue}'." },
					{ nameof(LengthAttribute), "'{PropertyName}' must be between {MinLengthValue} and {MaxLengthValue} characters." },
					{ nameof(LessThanAttribute), "'{PropertyName}' must be less than '{ComparisonValue}'." },
					{ nameof(LessThanOrEqualAttribute), "'{PropertyName}' must be less than or equal to '{ComparisonValue}'." },
					{ nameof(MatchAttribute), "'{PropertyName}' is not in the correct format." },
					{ nameof(MaxLengthAttribute), "'{PropertyName}' must be less than {MaxLengthValue} characters." },
					{ nameof(MinLengthAttribute), "'{PropertyName}' must be more than {MinLengthValue} characters." },
					{ nameof(NotEmptyAttribute), "'{PropertyName}' must not be empty." },
					{ nameof(NotEqualAttribute), "'{PropertyName}' must not be equal to '{ComparisonValue}'." },
					{ nameof(NotNullAttribute), "'{PropertyName}' must not be null." },
					{ nameof(OneOfAttribute), "'{PropertyName}' was not one of the specified values: {ValuesValue}." },
				}
			},
			{
				"fr",
				new Dictionary<string, string>
				{
					{ nameof(EmptyAttribute), "'{PropertyName}' doit être vide." },
					{ nameof(EnumValueAttribute), "'{PropertyName}' a une plage de valeurs qui n'inclut pas '{PropertyValue}'." },
					{ nameof(EqualAttribute), "'{PropertyName}' doit être égal à '{ComparisonValue}'." },
					{ nameof(GreaterThanAttribute), "'{PropertyName}' doit être supérieur à '{ComparisonValue}'." },
					{ nameof(GreaterThanOrEqualAttribute), "'{PropertyName}' doit être supérieur ou égal à '{ComparisonValue}'." },
					{ nameof(LengthAttribute), "'{PropertyName}' doit être compris entre {MinLengthValue} et {MaxLengthValue} caractères." },
					{ nameof(LessThanAttribute), "'{PropertyName}' doit être inférieur à '{ComparisonValue}'." },
					{ nameof(LessThanOrEqualAttribute), "'{PropertyName}' doit être inférieur ou égal à '{ComparisonValue}'." },
					{ nameof(MatchAttribute), "'{PropertyName}' n'est pas au bon format." },
					{ nameof(MaxLengthAttribute), "'{PropertyName}' doit être inférieur à {MaxLengthValue} caractères." },
					{ nameof(MinLengthAttribute), "'{PropertyName}' doit être supérieur à {MinLengthValue} caractères." },
					{ nameof(NotEmptyAttribute), "'{PropertyName}' ne doit pas être vide." },
					{ nameof(NotEqualAttribute), "'{PropertyName}' ne doit pas être égal à '{ComparisonValue}'." },
					{ nameof(NotNullAttribute), "'{PropertyName}' ne doit pas être nul." },
					{ nameof(OneOfAttribute), "'{PropertyName}' n'était pas l'une des valeurs spécifiées : {ValuesValue}." },
				}
			},
		}.ToDictionary(
			x => x.Key,
			x => x.Value.ToDictionary(
				y => y.Key,
				y => new LocalizedString(x.Key, y.Value)
		));
	}
}
