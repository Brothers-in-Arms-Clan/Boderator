using Discord;

namespace ArmaforcesMissionBot.Features.Emojis.Constants
{
    /// <summary>
    /// Built in Discord emojis.
    /// </summary>
    internal static class EmojiConstants
    {
        public static Emoji Ambulance { get; } = new Emoji("🚑");

        public static Emoji ArrowDownEmote { get; } = new Emoji("⬇");

        public static Emoji ArrowUpEmote { get; } = new Emoji("⬆");

        public static Emoji LockEmote { get; } = new Emoji("🔒");

        public static Emoji PinEmote { get; } = new Emoji("📍");

        public static Emoji ScissorsEmote { get; } = new Emoji("✂");
    }
}
