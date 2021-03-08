using System.Linq;
using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups.Missions.Extensions
{
    public static class MissionExtensions
    {
        public static bool IsUserSigned(this Mission mission, IUser user)
            => IsUserSigned(mission, user.Id);

        public static bool IsUserSigned(this Mission mission, ulong userId)
            => mission.SignedUsers.Any(signedUserId => signedUserId == userId);

        public static Result SignUser(this Mission mission, IUser user)
            => SignUser(mission, user.Id);

        public static Result SignUser(this Mission mission, ulong userId)
        {
            if (mission.IsUserSigned(userId))
            {
                return Result.Failure("This user is already signed to this mission.");
            }

            mission.SignedUsers.Add(userId);
            return Result.Success();
        }

        public static Result UnsignUser(this Mission mission, IUser user)
            => UnsignUser(mission, user.Id);

        public static Result UnsignUser(this Mission mission, ulong userId)
        {
            if (mission.IsUserSigned(userId))
            {
                mission.SignedUsers.Remove(userId);
            }

            return Result.Success();
        }
    }
}
