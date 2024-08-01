using Xunit;
using Xunit.Abstractions;

namespace Immediate.Validations.FunctionalTests.Common;

public sealed class IsolatedTestOrderer : ITestCollectionOrderer
{
	public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
	{
		var testCollectionsList = testCollections.ToList();
		var isolatedCollection = testCollectionsList.FirstOrDefault(tc => tc.DisplayName == nameof(IsolatedTestCollectionDefinition));
		var otherCollections = testCollectionsList.Where(tc => tc.DisplayName != nameof(IsolatedTestCollectionDefinition));

		if (isolatedCollection != null)
		{
			yield return isolatedCollection;
		}

		foreach (var collection in otherCollections)
		{
			yield return collection;
		}
	}
}
