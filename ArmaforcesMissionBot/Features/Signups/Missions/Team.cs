using System;
using System.Collections.Generic;

namespace ArmaforcesMissionBot.Features.Signups.Missions
{
    [Serializable]
    public class Team
    {
        public string Name;
        public string Pattern;
        public List<Slot> Slots = new List<Slot>();
        public ulong TeamMsg;
        public ulong Reserve = 0;
    }
}
