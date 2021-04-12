using System;
using ArmaForces.ArmaServerManager.Discord.Extensions;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Extensions;
using ArmaforcesMissionBot.Features.Emojis;
using ArmaforcesMissionBot.Features.Modsets;
using ArmaforcesMissionBot.Features.Modsets.Legacy;
using ArmaforcesMissionBot.Features.RichPresence;
using ArmaforcesMissionBot.Features.ServerManager;
using ArmaforcesMissionBot.Features.Signups;
using ArmaforcesMissionBot.Features.Signups.Missions.Slots;
using ArmaforcesMissionBot.Features.Signups.Missions.Validators;
using ArmaforcesMissionBot.Helpers;
using ArmaforcesMissionBot.Modules;
using ArmaforcesMissionBot.Providers.Guild;
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
                .AddConfig()
                .AddSingleton(CreateDiscordSocketClient)
                .AddSingleton<SignupsData>()
                .AddSingleton<OpenedDialogs>()
                .AddSingleton<MissionsArchiveData>()
                .AddSingleton<GameStatusUpdater>()
                .AddSingleton<MiscHelper>()
                .AddSingleton<SignupHelper>()
                .AddSingleton<BanHelper>()
                .AddSingleton<IGuildProviderFactory, GuildProviderFactory>()
                .AddSingleton(GuildProviderFactory.CreateGuildProvider)
                .AddTransient<IEmoteProvider, EmoteProvider>()
                .AddSignupsModules()
                .AddLogging()
                .AddHostedService<StartupService>()
                .AddSingleton<IModsetsApiClient, ModsetsApiClient>()
                .AddModsetProvider()
                .AddCommandModules()
                .AddServerManager<ServerManagerConfiguration>();

        private static IServiceCollection AddCommandModules(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddTransient<Bans>()
                .AddTransient<Misc>()
                .AddTransient<Mods>()
                .AddTransient<Ranks>()
                .AddTransient<Server>()
                .AddTransient<ServerConfig>()
                .AddTransient<Signups>()
                .AddTransient<Stats>();

        private static IServiceCollection AddModsetProvider(this IServiceCollection serviceCollection) => 
            serviceCollection.AddSingleton(provider =>
                string.IsNullOrWhiteSpace(provider.GetService<Config>().ModsetsApiUrl)
                    ? (IModsetProvider) new LegacyModsetProvider()
                    : new ModsetProvider(provider.GetService<IModsetsApiClient>()));

        private static IServiceCollection AddSignupsModules(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<ISlotFactory, SlotFactory>()
                .AddTransient<IMissionValidator, MissionValidator>()
                .AddTransient<ISignupsBuilder, SignupsBuilder>()
                .AddTransient<ISignupsLogic, SignupsLogic>()
                .AddSingleton<ISignupsBuilderDictionary, SignupsBuilderDictionary>()
                .AddTransient<ISignupsBuilderFactory, SignupsBuilderFactory>();
        }

        private static IServiceCollection AddConfig(this IServiceCollection serviceCollection)
        {
            var config = new Config();
            config.Load();
            return serviceCollection.AddSingleton(config);
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
