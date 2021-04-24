#nullable enable
using System;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Features.Signups.DTOs
{
    public class SignupsCreateRequestDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SignupsCloseTime { get; set; }

        [JsonProperty(Required = Required.Always)]
        public MissionDto Mission { get; set; } = new MissionDto();
    }
}
