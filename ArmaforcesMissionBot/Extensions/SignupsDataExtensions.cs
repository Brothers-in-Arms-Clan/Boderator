using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBotSharedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArmaforcesMissionBot.Extensions
{
    public static class SignupsDataExtensions
    {
        public static Mission GetCurrentlyEditedMission(this SignupsData signupsData, ulong userId)
        {
            return signupsData.Missions.SingleOrDefault(x =>
                x.Owner == userId &&
                ((x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New) ||
                (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started)));
        }
    }
}
