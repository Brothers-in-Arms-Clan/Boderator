using System;
using System.Threading.Tasks;
using ArmaForces.ArmaServerManager.Discord.Features.Server;
using ArmaforcesMissionBot.Attributes;
using Discord.Commands;

namespace ArmaforcesMissionBot.Modules
{
    [Name("ArmaServerManager - Server")]
    public class Server : ServerModule
    {
        public Server(IServerManagerClient serverManagerClient) : base(serverManagerClient)
        {
        }

        [Summary("Sprawdza status serwera.")]
        [ContextDMOrChannel]
        public override Task ServerStatus() => base.ServerStatus();

        [Summary("Pozwala natychmiast uruchomić serwer z zadanym modsetem. Na przykład: AF!startServer default.")]
        [ContextDMOrChannel]
        public override Task StartServer(string modsetName) => base.StartServer(modsetName);

        [Summary("Pozwala uruchomić serwer z zadanym modsetem o zadanej godzinie w danym dniu. Na przykład: AF!startServer default 2020-07-17T19:00.")]
        [ContextDMOrChannel]
        public override Task StartServer(string modsetName, DateTime? dateTime) => base.StartServer(modsetName, dateTime);
    }
}
