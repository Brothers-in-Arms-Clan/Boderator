using System;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ArmaForces.Boderator.BotService.Logging
{
    internal static class SerilogConfigurationExtensions
    {
        public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
            => hostBuilder.UseSerilog(ConfigureLogging());

        private static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging() => (context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration);
    }
}
