using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.BotService.Features.DiscordClient.Infrastructure.DependencyInjection
{
    internal static class DiscordServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscordClient(this IServiceCollection services)
        {
            return services
                .AddSingleton(DiscordSocketClientFactory.CreateDiscordClient)
                .AddSingleton<IDiscordService, DiscordService>()
                .AddHostedService<DiscordService>();
        }
    }
}
