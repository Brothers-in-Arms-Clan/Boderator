using System.Linq;
using ArmaforcesMissionBot.Features.Signups.Missions.Slots;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups.Missions.Extensions
{
    public static class TeamExtensions
    {
#nullable enable
        public static Slot? GetSlot(this Team team, IEmote emote)
#nullable restore
        {
            var reactionStringAnimatedVersion = emote.ToString()?.Insert(1, "a");

            return team.Slots.SingleOrDefault(
                teamSlot => Equals(teamSlot.Emoji, emote) || teamSlot.Emoji.ToString() == reactionStringAnimatedVersion);
        }
    }
}
