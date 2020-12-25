using Discord;

namespace ArmaforcesMissionBot.Features.Emojis
{
    internal interface IEmoteProvider
    {
        IEmote GetEmoteFromString(string emojiString);
    }
}
