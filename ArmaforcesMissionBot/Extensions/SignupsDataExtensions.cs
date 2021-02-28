using ArmaforcesMissionBot.DataClasses;
using System.Linq;
using ArmaforcesMissionBot.Features.Signups.Missions;

namespace ArmaforcesMissionBot.Extensions
{
    public static class SignupsDataExtensions
    {
        public static Mission GetCurrentlyEditedMission(this SignupsData signupsData, ulong userId)
        {
            return signupsData.Missions.SingleOrDefault(x =>
                x.Owner == userId &&
                ((x.Editing == Mission.EditEnum.New) ||
                (x.Editing == Mission.EditEnum.Started)));
        }
    }
}
