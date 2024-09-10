using Immediate.Validations.Shared;
using Xunit;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public partial class DuplicateTypeVisitTests
{
	[Validate]
	public partial interface IBaseInterface : IValidationTarget<IBaseInterface>
	{
		public static int VisitCount { get; set; }

		private static void AdditionalValidations(ValidationResult _, IBaseInterface __)
		{
			VisitCount++;
		}
	}

	[Validate]
	public partial interface ISubInterface : IBaseInterface, IValidationTarget<ISubInterface>;

	[Validate]
	public sealed partial record ValidateRecord : IBaseInterface, ISubInterface, IValidationTarget<ValidateRecord>;

	[Fact]
	public void TypeValidatorsAreVisitedAtMostOnce()
	{
		var record = new ValidateRecord();

		_ = ValidateRecord.Validate(record);

		Assert.Equal(1, IBaseInterface.VisitCount);
	}
}
