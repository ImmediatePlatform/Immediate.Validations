using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class SkipSelfTests
{
	[Validate]
	public partial record BaseClass : IValidationTarget<BaseClass>
	{
		public required string BaseString { get; init; }
	}

	[Validate(SkipSelf = true)]
	public sealed partial record SubClass : BaseClass, IValidationTarget<SubClass>
	{
		public required string IgnoredString { get; init; }
	}

	[Test]
	public void SkipSelfOperatesCorrectly()
	{
		var instance = new SubClass { BaseString = null!, IgnoredString = null!, };

		var errors = SubClass.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(BaseClass.BaseString),
					ErrorMessage = "'Base String' must not be null.",
				}
			],
			errors
		);
	}
}
