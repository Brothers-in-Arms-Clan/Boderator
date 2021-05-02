using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArmaForces.Boderator.BotService.Discord
{
    public sealed class DiscordService : IDiscordService, IHostedService
    {
        private readonly ILogger<DiscordService> _log;
        private readonly DiscordSocketClient _discordClient;
        private string _token;

        public DiscordService(ILogger<DiscordService> logger, string token)
        {
            _log = logger;
            _discordClient = new DiscordSocketClient();
            _discordClient.Log += message => Task.Run(() => _log.Log(MapSeverity(message.Severity), "[Discord.NET Log] " + message.Message));
            _token = token;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Discord Service started");
            await _discordClient.LoginAsync(TokenType.Bot, _token, true);
            await _discordClient.StartAsync();
            _discordClient.Connected += () => Task.Run(() => _log.LogInformation("Discord connected"));
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.Run(() =>
        {
            _discordClient.Dispose();
            _log.LogInformation("Discord Service stopped");
        }, cancellationToken);

        public string GetDiscordClientStatus()
        {
            string message = $"Current Discord Bot status: Login: {_discordClient.LoginState} | " +
                             $"Connection: {_discordClient.ConnectionState} | " +
                             $"Status: {_discordClient.Status}";
            _log.LogInformation(message);
            return message;
        }

        public async Task SetBotStatus(string newStatus, ActivityType statusType) =>
            await _discordClient.SetGameAsync(newStatus, type: statusType);

        private static LogLevel MapSeverity(LogSeverity severity) =>
            severity switch
            {
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical
            };
    }
}
