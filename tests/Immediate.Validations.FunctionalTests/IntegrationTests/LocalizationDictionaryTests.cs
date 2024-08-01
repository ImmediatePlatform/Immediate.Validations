using Immediate.Validations.Shared.Localization;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public class LocalizationDictionaryTests
{
	[Fact]
	public void InitializeWithGivenLocalizations()
	{
		var localizations = new Dictionary<string, string>
		{
			{ "Hello", "Hola" },
			{ "Goodbye", "Adiós" }
		};

		var localizationDictionary = new LocalizationDictionary(localizations);

		Assert.Equal(2, localizationDictionary.Count);
		Assert.True(localizationDictionary.ContainsKey("Hello"));
		Assert.True(localizationDictionary.ContainsKey("Goodbye"));
		Assert.Equal((string?)"Hola", (string?)localizationDictionary["Hello"].Value);
		Assert.Equal((string?)"Adiós", (string?)localizationDictionary["Goodbye"].Value);
	}

	[Fact]
	public void HandleEmptyDictionary()
	{
		var localizations = new Dictionary<string, string>();

		var localizationDictionary = new LocalizationDictionary(localizations);

		Assert.Empty(localizationDictionary);
	}

	[Fact]
	public void ThrowArgumentNullExceptionWhenLocalizationsIsNull()
	{
		Dictionary<string, string> localizations = null!;

		_ = Assert.Throws<ArgumentNullException>(() => new LocalizationDictionary(localizations));
	}

	[Fact]
	public void LocalizedStringsWithCorrectKeysAndValues()
	{
		var localizations = new Dictionary<string, string>
		{
			{ "Yes", "Sí" },
			{ "No", "No" }
		};

		var localizationDictionary = new LocalizationDictionary(localizations);

		Assert.True(localizationDictionary.ContainsKey("Yes"));
		Assert.True(localizationDictionary.ContainsKey("No"));
		Assert.Equal((string?)"Sí", (string?)localizationDictionary["Yes"].Value);
		Assert.Equal((string?)"No", (string?)localizationDictionary["No"].Value);
	}
}
