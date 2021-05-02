using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.BotService.Discord.DependencyInjection
{
    internal class DiscordServiceBuilder : IDiscordServiceBuilder
    {
        private IServiceCollection Services { get; }

        internal DiscordServiceBuilder(IServiceCollection services) => Services = services;
    }
}
