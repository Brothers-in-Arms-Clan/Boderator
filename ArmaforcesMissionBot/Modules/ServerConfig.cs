using System.Threading.Tasks;
using ArmaForces.ArmaServerManager.Discord.Features.ServerConfig;
using ArmaforcesMissionBot.Attributes;
using Discord.Commands;

namespace ArmaforcesMissionBot.Modules
{
    [Name("ArmaServerManager - Server Config")]
    public class ServerConfig : ServerConfigModule
    {
        public ServerConfig(IConfigurationManagerClient configurationManagerClient) : base(configurationManagerClient)
        {
        }

        [Summary("Pobiera config serwera dla danego modsetu.")]
        [ContextDMOrChannel]
        public override Task GetModsetConfig(string modsetName) => base.GetModsetConfig(modsetName);

        [Summary("Wrzuca config serwera dla danego modsetu.")]
        [ContextDMOrChannel]
        public override Task PutModsetConfig(string modsetName, string configContent = null) => base.PutModsetConfig(modsetName, configContent);

        [Summary("Pobiera główny config serwera.")]
        [ContextDMOrChannel]
        public override Task GetServerConfig() => base.GetServerConfig();

        [Summary("Wrzuca główny config serwera.")]
        [ContextDMOrChannel]
        public override Task PutServerConfig(string configContent = null) => base.PutServerConfig(configContent);
    }
}
