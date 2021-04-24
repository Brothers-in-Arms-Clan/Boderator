#nullable enable
using Discord;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Features.Signups.DTOs
{
    public class SlotDto
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public IEmote Emote { get; set; } = new Emoji(string.Empty);

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IUser? SignedUser { get; set; }
    }
}
