using ArmaforcesMissionBot.DataClasses;
using System.Linq;
using ArmaforcesMissionBot.Features.Signups.Missions;
using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Extensions
{
    public static class SignupsDataExtensions
    {
        public static Mission GetCurrentlyEditedMission(this SignupsData signupsData, ulong userId)
        {
            return signupsData.Missions.SingleOrDefault(mission =>
                mission.Owner == userId &&
                (mission.Editing == Mission.EditEnum.New ||
                mission.Editing == Mission.EditEnum.Started));
        }

#nullable enable
        public static Mission? GetCurrentlyCreatedMission(this SignupsData signupsData, ulong userId)
#nullable restore
        {
            return signupsData.Missions.SingleOrDefault(mission =>
                mission.Owner == userId &&
                mission.Editing == Mission.EditEnum.New);
        }

        public static Result<Mission> GetUserMissionForEdition(
            this SignupsData signupsData,
            IUser user,
            IGuildChannel channel)
        {
            var missionToBeEdited = signupsData.Missions.FirstOrDefault(x => x.SignupChannel == channel.Id);
            if (missionToBeEdited is null)
            {
                return Result.Failure<Mission>("There is no such mission.");
            }

            if (missionToBeEdited.Owner != user.Id)
            {
                return Result.Failure<Mission>("You cannot edit not owned missions.");
            }

            return Result.Success(missionToBeEdited);
        }
    }
}
