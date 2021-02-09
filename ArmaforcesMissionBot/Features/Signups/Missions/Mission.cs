using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Features.Signups.Missions
{
    [Serializable]
    public class Mission
    {
        [Serializable]
        public enum EditEnum
        {
            New,
            Started,
            NotEditing
        }
        public string Title;
        public DateTime Date;
        public DateTime? CloseTime = null;
        public string Description;
        public string Attachment;
        public byte[] AttachmentBytes;
        public string FileName;
        public string Modlist;
        public string ModlistUrl;
        public string ModlistName;
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
