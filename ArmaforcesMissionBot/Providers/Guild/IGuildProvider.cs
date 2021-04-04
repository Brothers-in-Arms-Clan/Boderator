using Discord;
using Discord.WebSocket;

namespace ArmaforcesMissionBot.Providers.Guild
{
    public interface IGuildProvider
    {
        IGuild GetGuild();

        SocketGuild GetSocketGuild();
    }
}
