using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.DiscordClient.DTOs;
using Discord;

namespace ArmaForces.Boderator.BotService.Features.DiscordClient
{
    internal interface IDiscordService
    {
        DiscordServiceStatusDto GetDiscordClientStatus();
        
        Task SetBotStatus(string newStatus, ActivityType statusType);
    }
}
