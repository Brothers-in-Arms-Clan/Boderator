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

        [Summary("Wrzuca config serwera dla danego modsetu. Config może być załączony w pliku *.json albo jako część wiadomości w formacie JSON po nazwie modsetu.")]
        [ContextDMOrChannel]
        public override Task PutModsetConfig(string modsetName, [Remainder] string configContent = null) => base.PutModsetConfig(modsetName, configContent);

        [Summary("Pobiera główny config serwera.")]
        [ContextDMOrChannel]
        public override Task GetServerConfig() => base.GetServerConfig();

        [Summary("Wrzuca główny config serwera. Config może być załączony w pliku *.json albo jako część wiadomości w formacie JSON.")]
        [ContextDMOrChannel]
        public override Task PutServerConfig([Remainder] string configContent = null) => base.PutServerConfig(configContent);
    }
}
