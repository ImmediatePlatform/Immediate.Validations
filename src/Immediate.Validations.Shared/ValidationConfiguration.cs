using Immediate.Validations.Shared.Localization;
using Microsoft.Extensions.Localization;

namespace Immediate.Validations.Shared;

/// <summary>
/// Provides configuration for validation localization.
/// </summary>
public static class ValidationConfiguration
{
	/// <summary>
	///	    Gets or sets the <see cref="IStringLocalizer"/> used for validation messages.
	/// </summary>
	/// <value>
	///	    An instance of <see cref="IStringLocalizer"/> used to localize validation messages.
	/// </value>
	public static IStringLocalizer Localizer { get; set; } = new ValidatorLocalizer();
}
