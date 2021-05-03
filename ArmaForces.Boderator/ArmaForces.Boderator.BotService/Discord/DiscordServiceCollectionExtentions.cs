using ArmaForces.Boderator.BotService.Discord.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArmaForces.Boderator.BotService.Discord
{
    internal static class DiscordServiceCollectionExtentions
    {
        public static IDiscordServiceBuilder AddDiscordService(this IServiceCollection serviceDescriptors, string token)
        {
            serviceDescriptors.AddSingleton<IDiscordService, DiscordService>(sP => new DiscordService(sP.GetService<ILogger<DiscordService>>(), token));
            serviceDescriptors.AddHostedService(sP => sP.GetRequiredService<IDiscordService>() as DiscordService);
            return new DiscordServiceBuilder(serviceDescriptors);
        }
    }
}
