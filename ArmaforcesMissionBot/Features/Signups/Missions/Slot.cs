using System.Collections.Generic;

namespace ArmaforcesMissionBot.Features.Signups.Missions
{
    public class Slot
    {
        public string Name;
        public string Emoji; // slotID
        public int Count;
        public List<ulong> Signed = new List<ulong>();

        public Slot()
        {
            Name = "";
            Emoji = "";
            Count = 0;
        }

        public Slot(string emoji, int count)
        {
            Name = "";
            Emoji = emoji;
            Count = count;
        }

        public Slot(
            string name,
            string emoji,
            int count)
        {
            Name = name;
            Emoji = emoji;
            Count = count;
        }
    }
}
