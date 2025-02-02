using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.Validators;

public sealed partial class EnumValueTests
{
	public enum ValidState
	{
		None = 0,
		Valid,
		Invalid,
	}

	[Flags]
	public enum FlagsState
	{
		None = 0,
		Valid = 0x1,
		Invalid = 0x2,
	}

	[Validate]
	public partial record EnumRecord : IValidationTarget<EnumRecord>
	{
		public required ValidState ValidState { get; init; }
	}

	[Validate]
	public partial record FlagsEnumRecord : IValidationTarget<FlagsEnumRecord>
	{
		public required FlagsState ValidState { get; init; }
	}

	[Test]
	public void EnumValidValue()
	{
		var instance = new EnumRecord { ValidState = ValidState.Valid };

		var errors = EnumRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void EnumInvalidValue()
	{
		var instance = new EnumRecord { ValidState = (ValidState)3 };

		var errors = EnumRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(EnumRecord.ValidState),
					ErrorMessage = "'Valid State' has a range of values which does not include '3'.",
				},
			],
			errors
		);
	}

	[Test]
	public void FlagsEnumValidValue()
	{
		var instance = new FlagsEnumRecord { ValidState = FlagsState.Valid };

		var errors = FlagsEnumRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void FlagsEnumValidFlags()
	{
		var instance = new FlagsEnumRecord { ValidState = FlagsState.Valid | FlagsState.Invalid };

		var errors = FlagsEnumRecord.Validate(instance);

		Assert.Empty(errors);
	}

	[Test]
	public void FlagsEnumInvalidValue()
	{
		var instance = new FlagsEnumRecord { ValidState = (FlagsState)4 };

		var errors = FlagsEnumRecord.Validate(instance);

		Assert.Equal(
			[
				new()
				{
					PropertyName = nameof(EnumRecord.ValidState),
					ErrorMessage = "'Valid State' has a range of values which does not include '4'.",
				},
			],
			errors
		);
	}
}
