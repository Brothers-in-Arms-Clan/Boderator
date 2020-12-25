using System.Collections.Generic;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups.Missions.Slots
{
    public class Slot
    {
        public string Name { get; set; }

        /// <summary>
        /// Used as SlotID.
        /// </summary>
        public IEmote Emoji { get; }

        public int Count { get; } = 0;

        public List<ulong> Signed { get; } = new List<ulong>();

        public Slot(
            string name,
            IEmote emoji,
            int count)
        {
            Name = name;
            Emoji = emoji;
            Count = count;
        }
    }
}
