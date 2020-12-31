using ArmaforcesMissionBot.DataClasses;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ArmaforcesMissionBot.Attributes
{
    public enum RanksEnum
    {
        Recruiter
    }

    internal static class RanksEnumMethods
    {
        public static ulong GetID(this RanksEnum role, Config config)
            => role switch
            {
                RanksEnum.Recruiter => config.RecruiterRole,
                _ => 0
            };
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RequireRankAttribute : PreconditionAttribute
    {
        private readonly RanksEnum _role;

        public RequireRankAttribute(RanksEnum role)
        {
            _role = role;
        }

        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var config = services.GetService<Config>();

            return ((SocketGuildUser) context.User).Roles.Any(x => x.Id == _role.GetID(config))
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("Co ty próbujesz osiągnąć?");
        }
    }
}
