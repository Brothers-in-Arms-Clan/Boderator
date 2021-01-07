using ArmaForces.ArmaServerManager.Discord.Configuration;
using ArmaforcesMissionBot.DataClasses;

namespace ArmaforcesMissionBot.Features.ServerManager
{
    public class ServerManagerConfiguration : IManagerConfiguration
    {
        public string ServerManagerUrl { get; }
        public string ServerManagerApiKey { get; }

        public ServerManagerConfiguration(Config config)
        {
            ServerManagerUrl = config.ServerManagerUrl;
            ServerManagerApiKey = config.ServerManagerApiKey;
        }
    }
}
