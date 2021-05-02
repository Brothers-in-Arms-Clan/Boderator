using System.Threading.Tasks;
using Discord;

namespace ArmaForces.Boderator.BotService.Discord
{
    public interface IDiscordService
    {
        string GetDiscordClientStatus();
        Task SetBotStatus(string newStatus, ActivityType statusType = ActivityType.CustomStatus);
    }
}
