using ArmaForces.Boderator.BotService.Tests.TestUtilities.TestFixtures;
using Xunit;

namespace ArmaForces.Boderator.BotService.Tests.TestUtilities.Collections.Definitions
{
    [CollectionDefinition(CollectionsNames.ApiTest)]
    public class TestApiCollection : ICollectionFixture<TestApiServiceFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
