#nullable enable
using System;
using System.Collections.Generic;
using Discord;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Features.Signups.DTOs
{
    public class MissionDto
    {
        [JsonProperty(Required = Required.Always)]
        public IUser Creator { get; set; } = null!;

        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Image { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ModsetName { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public DateTime Date { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<TeamDto> Teams { get; set; } = new List<TeamDto>();
    }
}
