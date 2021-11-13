using System;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.DiscordClient.Infrastructure.Logging;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArmaForces.Boderator.BotService.Features.DiscordClient.Infrastructure.DependencyInjection
{
    internal static class DiscordSocketClientFactory
    {
        private static bool _isClientCreated;
        
        public static DiscordSocketClient CreateDiscordClient(IServiceProvider serviceProvider)
        {
            if (_isClientCreated)
                throw new InvalidOperationException("Only one Discord client can be created!");

            _isClientCreated = true;
            
            var discordSocketConfig = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100000
            };
            var client = new DiscordSocketClient(discordSocketConfig);
            
            ConfigureLogging(client, serviceProvider);

            return client;
        }

        private static void ConfigureLogging(DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DiscordSocketClient>>();
            client.Log += message => Task.Run(() => message.LogWithLoggerAsync(logger));
            client.Connected += () => Task.Run(() => logger.LogInformation("Discord connected"));
        }
    }
}
