using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class EnumValueTests
{
	public enum ValidState
	{
		None = 0,
		Valid,
		Invalid,
	}

	[Validate]
	public partial record EnumRecord : IValidationTarget<EnumRecord>
	{
		public required ValidState ValidState { get; init; }
	}

	[Fact]
	public void StringNotNullTestWhenNotNull()
	{
		var instance = new EnumRecord { ValidState = ValidState.Valid };

		var errors = EnumRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Fact]
	public void StringNotNullTestWhenNull()
	{
		var instance = new EnumRecord { ValidState = (ValidState)3 };

		var errors = EnumRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(EnumRecord.ValidState),
					ErrorMessage = "'Valid State' has a range of values which does not include '3'.",
				}
			],
			errors
		);
	}
}
