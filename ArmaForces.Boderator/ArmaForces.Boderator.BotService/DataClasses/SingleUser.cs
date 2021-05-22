using System.Collections.Generic;

namespace ArmaForces.Boderator.BotService.DataClasses
{
    public class SingleUser
    {
        public ulong UserID;
        public List<DLC> DLCList = new List<DLC>();

        public enum DLC
        {
            Karts,
            Helicopters,
            Marksman,
            Apex,
            Jets,
            LawsOfWar,
            Tanks,
            Contact,
            GlobalMobilization,
            SOGPrairieFire,
            CSLA
        }

    }
}
