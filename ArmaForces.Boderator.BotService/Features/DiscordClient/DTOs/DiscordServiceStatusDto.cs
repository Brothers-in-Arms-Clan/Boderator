using Discord;

namespace ArmaForces.Boderator.BotService.Features.DiscordClient.DTOs
{
    public class DiscordServiceStatusDto
    {
        public ConnectionState ConnectionState { get; init; }
        public LoginState LoginState { get; init; }
        public UserStatus ClientState { get; init; }
    }
}
