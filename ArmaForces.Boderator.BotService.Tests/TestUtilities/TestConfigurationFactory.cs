using ArmaForces.Boderator.BotService.Configuration;
using ArmaForces.Boderator.Core.Tests.TestUtilities;

namespace ArmaForces.Boderator.BotService.Tests.TestUtilities
{
    internal class TestConfigurationFactory : IBoderatorConfigurationFactory
    {
        public BoderatorConfiguration CreateConfiguration() => new BoderatorConfiguration
        {
            ConnectionString = TestDatabaseConstants.TestConnectionString,
            DiscordToken = string.Empty
        };
    }
}
