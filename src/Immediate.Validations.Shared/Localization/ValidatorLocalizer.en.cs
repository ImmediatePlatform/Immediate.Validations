using Microsoft.Extensions.Localization;

namespace Immediate.Validations.Shared.Localization;

internal sealed partial class ValidatorLocalizer : IStringLocalizer
{
	private static Dictionary<string, LocalizedString> En() => new LocalizationDictionary(new()
	{
		[nameof(EmptyAttribute)] = "'{PropertyName}' must be empty.",
		[nameof(EnumValueAttribute)] = "'{PropertyName}' has a range of values which does not include '{PropertyValue}'.",
		[nameof(EqualAttribute)] = "'{PropertyName}' must be equal to '{ComparisonValue}'.",
		[nameof(GreaterThanAttribute)] = "'{PropertyName}' must be greater than '{ComparisonValue}'.",
		[nameof(GreaterThanOrEqualAttribute)] = "'{PropertyName}' must be greater than or equal to '{ComparisonValue}'.",
		[nameof(LengthAttribute)] = "'{PropertyName}' must be between {MinLengthValue} and {MaxLengthValue} characters.",
		[nameof(LessThanAttribute)] = "'{PropertyName}' must be less than '{ComparisonValue}'.",
		[nameof(LessThanOrEqualAttribute)] = "'{PropertyName}' must be less than or equal to '{ComparisonValue}'.",
		[nameof(MatchAttribute)] = "'{PropertyName}' is not in the correct format.",
		[nameof(MaxLengthAttribute)] = "'{PropertyName}' must be less than {MaxLengthValue} characters.",
		[nameof(MinLengthAttribute)] = "'{PropertyName}' must be more than {MinLengthValue} characters.",
		[nameof(NotEmptyAttribute)] = "'{PropertyName}' must not be empty.",
		[nameof(NotEqualAttribute)] = "'{PropertyName}' must not be equal to '{ComparisonValue}'.",
		[nameof(NotNullAttribute)] = "'{PropertyName}' must not be null.",
		[nameof(OneOfAttribute)] = "'{PropertyName}' was not one of the specified values: {ValuesValue}.",
	});

}
