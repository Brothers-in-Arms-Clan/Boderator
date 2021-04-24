#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Features.Signups.DTOs
{
    public class TeamDto
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public List<SlotDto> Slots { get; set; } = new List<SlotDto>();
    }
}
