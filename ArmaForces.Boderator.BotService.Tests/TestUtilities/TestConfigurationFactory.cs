using ArmaForces.Boderator.BotService.Configuration;

namespace ArmaForces.Boderator.BotService.Tests.TestUtilities
{
    internal class TestConfigurationFactory : IBoderatorConfigurationFactory
    {
        public BoderatorConfiguration CreateConfiguration() => new BoderatorConfiguration
        {
            DiscordToken = string.Empty
        };
    }
}
