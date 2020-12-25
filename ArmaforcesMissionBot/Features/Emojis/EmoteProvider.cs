using System;
using Discord;

namespace ArmaforcesMissionBot.Features.Emojis
{
    internal class EmoteProvider : IEmoteProvider
    {
        public IEmote GetEmoteFromString(string emojiString)
        {
            // TODO: Get list of custom emotes from context.Guild.Emotes
            try
            {
                return GetCustomEmoji(emojiString);
            }
            catch (ArgumentException)
            {
                return GetStandardEmoji(emojiString);
            }
        }

        private static Emote GetCustomEmoji(string emojiName)
        {
            return Emote.Parse(emojiName);
        }

        private static Emoji GetStandardEmoji(string emojiName) => new Emoji(emojiName);
    }
}
