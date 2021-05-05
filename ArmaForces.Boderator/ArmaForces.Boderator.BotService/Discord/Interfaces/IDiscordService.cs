using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.DTOs;
using Discord;

namespace ArmaForces.Boderator.BotService.Discord
{
    public interface IDiscordService
    {
        DiscordServiceStatus GetDiscordClientStatus();
        Task SetBotStatus(string newStatus, ActivityType statusType);
    }
}
