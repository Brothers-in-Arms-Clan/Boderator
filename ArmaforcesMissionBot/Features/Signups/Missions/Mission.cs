using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Features.Signups.Missions
{
    [Serializable]
    public class Mission : IMission
    {
        [Serializable]
        public enum EditEnum
        {
            New,
            Started,
            NotEditing
        }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CloseTime { get; set; } = null;
        public string Description { get; set; }

        public string Attachment;
        public byte[] AttachmentBytes;
        public string FileName;
        public string Modlist { get; set; }
        public string ModlistUrl;
        public string ModlistName;
        public ulong FreeSlots { get; set; }
        public ulong AllSlots { get; set; }
        public List<Team> Teams = new List<Team>();
        public ulong Owner;
        public ulong SignupChannel;
        public List<ulong> SignedUsers = new List<ulong>();
        [NonSerialized]
        [JsonIgnore]
        public EditEnum Editing = EditEnum.NotEditing;
        [NonSerialized]
        [JsonIgnore]
        public ulong EditTeamsMessage = 0;
        [NonSerialized]
        [JsonIgnore]
        public int HighlightedTeam = 0;
        [NonSerialized]
        [JsonIgnore]
        public bool IsMoving = false;
        [NonSerialized]
        [JsonIgnore]
        public SemaphoreSlim Access = new SemaphoreSlim(1);
        [NonSerialized]
        [JsonIgnore]
        public bool CustomClose = false;

        [NonSerialized]
        [JsonIgnore]
        public bool MentionEveryone = true;
    }
}
