using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.BotService.Discord.DependencyInjection
{
    internal class DiscordServiceBuilder : IDiscordServiceBuilder
    {
        private IServiceCollection Services { get; }

        public DiscordServiceBuilder(IServiceCollection services) => Services = services;
    }
}
