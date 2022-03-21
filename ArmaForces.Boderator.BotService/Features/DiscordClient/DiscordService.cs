using System.Threading;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Configuration;
using ArmaForces.Boderator.BotService.Features.DiscordClient.DTOs;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArmaForces.Boderator.BotService.Features.DiscordClient
{
    internal sealed class DiscordService : IDiscordService, IHostedService
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger<DiscordService> _logger;
        
        private readonly string _token;

        public DiscordService(
            DiscordSocketClient discordClient,
            BoderatorConfiguration configuration,
            ILogger<DiscordService> logger)
        {
            _logger = logger;
            _discordClient = discordClient;
            _token = configuration.DiscordToken;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord Service started");
            // await _discordClient.LoginAsync(TokenType.Bot, _token);
            // await _discordClient.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.Run(() =>
        {
            _discordClient.Dispose();
            _logger.LogInformation("Discord Service stopped");
        }, cancellationToken);

        public DiscordServiceStatusDto GetDiscordClientStatus()
        {
            _logger.LogInformation(
                "Current Discord Bot status: Login: {BotStatus} | " +
                "Connection: {ConnectionStatus} | " +
                "Status: {ClientStatus}",
                _discordClient.LoginState,
                _discordClient.ConnectionState,
                _discordClient.Status);
            
            return new DiscordServiceStatusDto
            {
                ConnectionState = _discordClient.ConnectionState,
                LoginState = _discordClient.LoginState,
                ClientState = _discordClient.Status
            };
        }

        public async Task SetBotStatus(string newStatus, ActivityType statusType) =>
            await _discordClient.SetGameAsync(newStatus, type: statusType);
    }
}
