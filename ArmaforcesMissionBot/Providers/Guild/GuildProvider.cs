using Discord;
using Discord.WebSocket;

namespace ArmaforcesMissionBot.Providers.Guild
{
    public class GuildProvider : IGuildProvider
    {
        private readonly SocketGuild _guild;

        public GuildProvider(SocketGuild socketGuild)
        {
            _guild = socketGuild;
        }

        public IGuild GetGuild() => GetSocketGuild();

        public SocketGuild GetSocketGuild() => _guild;
    }
}
