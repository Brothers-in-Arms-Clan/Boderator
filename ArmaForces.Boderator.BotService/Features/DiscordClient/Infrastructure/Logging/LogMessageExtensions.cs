using System;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

// Disabled warning as messages are coming from Discord logger
// ReSharper disable TemplateIsNotCompileTimeConstantProblem
namespace ArmaForces.Boderator.BotService.Features.DiscordClient.Infrastructure.Logging
{
    internal static class LogMessageExtensions
    {
        public static Task LogWithLoggerAsync(this LogMessage logMessage, ILogger logger)
        {
            LogWithLogger(logMessage, logger);
            return Task.CompletedTask;
        }

        private static void LogWithLogger(this LogMessage logMessage, ILogger logger)
        {
            if (logMessage.Exception != null)
            {
                LogExceptionWithLogger(logMessage, logger);
                return;
            }

            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    logger.LogCritical(logMessage.Message);
                    break;
                case LogSeverity.Error:
                    logger.LogError(logMessage.Message);
                    break;
                case LogSeverity.Warning:
                    logger.LogWarning(logMessage.Message);
                    break;
                case LogSeverity.Info:
                    logger.LogInformation(logMessage.Message);
                    break;
                case LogSeverity.Verbose:
                    logger.LogDebug(logMessage.Message);
                    break;
                case LogSeverity.Debug:
                    logger.LogTrace(logMessage.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void LogExceptionWithLogger(this LogMessage logMessage, ILogger logger)
        {
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    logger.LogCritical(logMessage.Exception, logMessage.Message);
                    break;
                case LogSeverity.Error:
                    logger.LogError(logMessage.Exception, logMessage.Message);
                    break;
                case LogSeverity.Warning:
                    logger.LogWarning(logMessage.Exception, logMessage.Message);
                    break;
                case LogSeverity.Info:
                    logger.LogInformation(logMessage.Exception, logMessage.Message);
                    break;
                case LogSeverity.Verbose:
                    logger.LogDebug(logMessage.Exception, logMessage.Message);
                    break;
                case LogSeverity.Debug:
                    logger.LogTrace(logMessage.Exception, logMessage.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
