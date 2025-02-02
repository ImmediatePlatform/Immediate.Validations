using System.Globalization;
using Immediate.Validations.Shared;
using Immediate.Validations.Shared.Localization;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed class ValidatorLocalizerTests
{
	[Test]
	public void GetAllStringsReturnsCorrectLocalizationsForCurrentCulture()
	{
		var localizer = new ValidatorLocalizer();
		CultureInfo.CurrentUICulture = new CultureInfo("en");

		var strings = localizer.GetAllStrings(includeParentCultures: false).ToList();

		Assert.NotNull(strings);
		Assert.Equal(15, strings.Count);
		Assert.Contains(
			strings,
			s => string.Equals(s.Name, nameof(EmptyAttribute), StringComparison.Ordinal)
				&& string.Equals(s.Value, "'{PropertyName}' must be empty.", StringComparison.Ordinal));
	}

	[Test]
	public void IndexerReturnsCorrectLocalizedStringForCurrentCulture()
	{
		var localizer = new ValidatorLocalizer();
		CultureInfo.CurrentUICulture = new CultureInfo("fr");

		var localizedString = localizer[nameof(EmptyAttribute)];

		Assert.NotNull(localizedString);
		Assert.Equal(nameof(EmptyAttribute), localizedString.Name);
		Assert.Equal("'{PropertyName}' doit être vide.", localizedString.Value);
	}

	[Test]
	public void IndexerReturnsNameWhenLocalizationNotFound()
	{
		var localizer = new ValidatorLocalizer();
		CultureInfo.CurrentUICulture = new CultureInfo("fr");

		var localizedString = localizer["NonExistentKey"];

		Assert.NotNull(localizedString);
		Assert.Equal("NonExistentKey", localizedString.Name);
		Assert.Equal("NonExistentKey", localizedString.Value);
		Assert.True(localizedString.ResourceNotFound);
	}

	[Test]
	public void IndexerFallsBackToEnglishWhenCultureNotFound()
	{
		var localizer = new ValidatorLocalizer();
		CultureInfo.CurrentUICulture = new CultureInfo("es");

		var localizedString = localizer[nameof(EmptyAttribute)];

		Assert.NotNull(localizedString);
		Assert.Equal(nameof(EmptyAttribute), localizedString.Name);
		Assert.Equal("'{PropertyName}' must be empty.", localizedString.Value);
	}

	[Test]
	public void IndexerWithArgumentsReturnsCorrectLocalizedString()
	{
		var localizer = new ValidatorLocalizer();
		CultureInfo.CurrentUICulture = new CultureInfo("en");

		var localizedString = localizer[nameof(EmptyAttribute), "arg1", "arg2"];

		Assert.NotNull(localizedString);
		Assert.Equal(nameof(EmptyAttribute), localizedString.Name);
		Assert.Equal("'{PropertyName}' must be empty.", localizedString.Value);
	}
}
