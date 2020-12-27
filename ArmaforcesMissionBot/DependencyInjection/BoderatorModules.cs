using System;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Extensions;
using ArmaforcesMissionBot.Features.RichPresence;
using ArmaforcesMissionBot.Helpers;
using ArmaforcesMissionBot.Services;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArmaforcesMissionBot.DependencyInjection
{
    public static class BoderatorModules
    {
        public static IServiceCollection AddBoderatorModules(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton(CreateDiscordSocketClient)
                .AddSingleton(CreateConfig)
                .AddSingleton<SignupsData>()
                .AddSingleton<OpenedDialogs>()
                .AddSingleton<MissionsArchiveData>()
                .AddSingleton<GameStatusUpdater>()
                .AddSingleton<MiscHelper>()
                .AddSingleton<SignupHelper>()
                .AddSingleton<BanHelper>()
                .AddLogging()
                .AddHostedService<StartupService>();

        private static Config CreateConfig(IServiceProvider serviceCollection)
        {
            var config = new Config();
            config.Load();
            return config;
        }

        private static DiscordSocketClient CreateDiscordSocketClient(IServiceProvider provider)
        {
            var discordSocketConfig = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100000
            };
            var client = new DiscordSocketClient(discordSocketConfig);
            var logger = provider.GetService<ILogger<DiscordSocketClient>>();
            client.Log += message => message.LogWithLoggerAsync(logger);
            return client;
        }
    }
}
