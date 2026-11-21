using Immediate.Validations.Shared;

namespace Immediate.Validations.FunctionalTests.IntegrationTests;

public sealed partial class PartialRecordTests
{
	public sealed partial record RenameCommand
	{
		[MinLength(3)]
		public required string Reason { get; init; }
	}
}
