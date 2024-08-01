using Xunit;
using Xunit.Abstractions;

namespace Immediate.Validations.FunctionalTests.Common;

public sealed class IsolatedTestOrderer : ITestCollectionOrderer
{
	public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections) =>
		testCollections.OrderBy(tc => tc.DisplayName == nameof(IsolatedTestCollectionDefinition));
}
