using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Features.Signups.Missions;

namespace ArmaforcesMissionBot.DataClasses
{
    public class MissionsArchiveData
    {
        public class Mission : IMission
        {
            public string Title { get; set; }
            public DateTime Date { get; set; }
            public DateTime? CloseTime { get; set; } = null;
            public string Description { get; set; }
            public string Modlist { get; set; }
            public string ModlistUrl;
            public string ModlistName;
            public string Attachment;
            public ulong FreeSlots { get; set; }
            public ulong AllSlots { get; set; }
        }

        public List<Mission> ArchiveMissions = new List<Mission>();
    }
}
