using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class PartialRecordTests
{
	[Validate]
	public sealed partial record RenameCommand : IValidationTarget<RenameCommand>
	{
		[NotEmpty]
		public required string Name { get; init; }
	}

	[Theory]
	[InlineData("", "valid", nameof(RenameCommand.Name))]
	[InlineData("valid", "no", nameof(RenameCommand.Reason))]
	public void ReportsErrorsFromBothDeclarations(string name, string reason, string propertyName)
	{
		var command = new RenameCommand { Name = name, Reason = reason };

		var error = Assert.Single(RenameCommand.Validate(command));

		Assert.Equal(propertyName, error.PropertyName);
	}
}
