using Xunit;

namespace Immediate.Validations.FunctionalTests.Common;

[CollectionDefinition(nameof(IsolatedTestCollectionDefinition), DisableParallelization = true)]
public sealed class IsolatedTestCollectionDefinition : ICollectionFixture<IsolatedTestFixture>;

public sealed class IsolatedTestFixture;
