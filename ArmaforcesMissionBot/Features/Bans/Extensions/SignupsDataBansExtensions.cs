using System;
using ArmaforcesMissionBot.DataClasses;
using Discord;

namespace ArmaforcesMissionBot.Features.Bans.Extensions
{
    public static class SignupsDataBansExtensions
    {
        public static bool UserHasBan(this SignupsData signupsData, IUser user, DateTime missionDate)
            => UserHasBan(signupsData, user.Id, missionDate);

        public static bool UserHasBan(this SignupsData signupsData, ulong userId, DateTime missionDate)
        {
            return signupsData.SignupBans.ContainsKey(userId)
                   && signupsData.SignupBans[userId] > missionDate;
        }
    }
}
