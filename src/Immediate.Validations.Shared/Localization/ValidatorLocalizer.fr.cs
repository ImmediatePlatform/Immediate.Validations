using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Localization;

namespace Immediate.Validations.Shared.Localization;

internal sealed partial class ValidatorLocalizer : IStringLocalizer
{
	[ExcludeFromCodeCoverage]
	private static Dictionary<string, LocalizedString> Fr() =>
		new LocalizationDictionary(new(StringComparer.Ordinal)
		{
			[nameof(EmptyAttribute)] = "'{PropertyName}' doit être vide.",
			[nameof(EnumValueAttribute)] = "'{PropertyName}' a une plage de valeurs qui n'inclut pas '{PropertyValue}'.",
			[nameof(EqualAttribute)] = "'{PropertyName}' doit être égal à '{ComparisonValue}'.",
			[nameof(GreaterThanAttribute)] = "'{PropertyName}' doit être supérieur à '{ComparisonValue}'.",
			[nameof(GreaterThanOrEqualAttribute)] = "'{PropertyName}' doit être supérieur ou égal à '{ComparisonValue}'.",
			[nameof(LengthAttribute)] = "'{PropertyName}' doit être compris entre {MinLengthValue} et {MaxLengthValue} caractères.",
			[nameof(LessThanAttribute)] = "'{PropertyName}' doit être inférieur à '{ComparisonValue}'.",
			[nameof(LessThanOrEqualAttribute)] = "'{PropertyName}' doit être inférieur ou égal à '{ComparisonValue}'.",
			[nameof(MatchAttribute)] = "'{PropertyName}' n'est pas au bon format.",
			[nameof(MaxLengthAttribute)] = "'{PropertyName}' doit être inférieur à {MaxLengthValue} caractères.",
			[nameof(MinLengthAttribute)] = "'{PropertyName}' doit être supérieur à {MinLengthValue} caractères.",
			[nameof(NotEmptyAttribute)] = "'{PropertyName}' ne doit pas être vide.",
			[nameof(NotEqualAttribute)] = "'{PropertyName}' ne doit pas être égal à '{ComparisonValue}'.",
			[nameof(NotNullAttribute)] = "'{PropertyName}' ne doit pas être nul.",
			[nameof(OneOfAttribute)] = "'{PropertyName}' n'était pas l'une des valeurs spécifiées : {ValuesValue}.",
		});
}
