using ArmaforcesMissionBot.DataClasses;
using System.Linq;
using ArmaforcesMissionBot.Features.Signups.Missions;

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
    }
}
