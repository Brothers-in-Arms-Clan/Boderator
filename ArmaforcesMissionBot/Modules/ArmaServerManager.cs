using System;
using System.Threading.Tasks;
using ArmaForces.ArmaServerManager.Discord.Features.Mods;
using ArmaForces.ArmaServerManager.Discord.Features.Server;
using ArmaForces.ArmaServerManager.Discord.Features.ServerConfig;
using ArmaforcesMissionBot.Attributes;
using Discord.Commands;

namespace ArmaforcesMissionBot.Modules
{
    public class ArmaServerManager : ModuleBase<SocketCommandContext>
    {
        private readonly ServerConfigModule _serverConfigModule;
        private readonly ServerModule _serverModule;
        private readonly ModsModule _modsModule;

        public ArmaServerManager(
            ServerConfigModule serverConfigModule,
            ServerModule serverModule,
            ModsModule modsModule)
        {
            _serverConfigModule = serverConfigModule;
            _serverModule = serverModule;
            _modsModule = modsModule;
        }

        [Command("startServer")]
        [Summary("Pozwala uruchomić serwer z zadanym modsetem o zadanej godzinie w danym dniu. Na przykład: AF!startServer default 2020-07-17T19:00.")]
        [ContextDMOrChannel]
        public async Task StartServer(string modsetName)
            => await _serverModule.StartServer(modsetName, null);

        [Command("startServer")]
        [Summary("Pozwala uruchomić serwer z zadanym modsetem o zadanej godzinie w danym dniu. Na przykład: AF!startServer default 2020-07-17T19:00.")]
        [ContextDMOrChannel]
        public async Task StartServer(string modsetName, DateTime? dateTime)
            => await _serverModule.StartServer(modsetName, dateTime);

        [Command("serverStatus")]
        [Summary("Sprawdza status serwera.")]
        public async Task ServerStatus() => await _serverModule.ServerStatus();

        [Command("updateMods")]
        [Summary("Pozwala zaplanować aktualizację wszystkich modyfikacji lub wybranego modsetu.")]
        [ContextDMOrChannel]
        public async Task UpdateMods(DateTime? scheduleAt = null)
            => await _modsModule.UpdateMods(null, scheduleAt);

        [Command("updateMods")]
        [Summary("Pozwala zaplanować aktualizację wszystkich modyfikacji lub wybranego modsetu.")]
        [ContextDMOrChannel]
        public async Task UpdateMods(string modsetName = null, DateTime? scheduleAt = null)
            => await _modsModule.UpdateMods(modsetName, scheduleAt);

        [Command("modsetConfig")]
        [Summary("Pobiera config serwera dla danego modsetu.")]
        [ContextDMOrChannel]
        public async Task ModsetConfig(string modsetName) => await _serverConfigModule.ModsetConfig(modsetName);

        [Command("putModsetConfig")]
        [Summary("Wrzuca config serwera dla danego modsetu.")]
        [ContextDMOrChannel]
        public async Task PutModsetConfig(string modsetName, [Remainder] string configContent = null)
            => await _serverConfigModule.PutModsetConfig(modsetName, configContent);

        [Command("serverConfig")]
        [Summary("Pobiera główny config serwera.")]
        [ContextDMOrChannel]
        public async Task ServerConfig() => await _serverConfigModule.ServerConfig();

        [Command("putServerConfig")]
        [Summary("Wrzuca główny config serwera.")]
        [ContextDMOrChannel]
        public async Task PutServerConfig([Remainder] string configContent = null)
            => await _serverConfigModule.PutServerConfig(configContent);
    }
}
