using System;
using ArmaforcesMissionBot.DataClasses;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaforcesMissionBot.Providers.Guild
{
    public class GuildProviderFactory : IGuildProviderFactory
    {
        private readonly DiscordSocketClient _client;
        private readonly Config _config;

        public GuildProviderFactory(DiscordSocketClient client, Config config)
        {
            _client = client;
            _config = config;
        }

        public IGuildProvider CreateGuildProvider()
        {
            return new GuildProvider(_client.GetGuild(_config.AFGuild));
        }

        public static IGuildProvider CreateGuildProvider(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IGuildProviderFactory>().CreateGuildProvider();
        }
    }
}
