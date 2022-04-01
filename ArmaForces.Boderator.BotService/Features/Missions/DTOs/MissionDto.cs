using System;
using Newtonsoft.Json;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

public record MissionDto
{
    public long MissionId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }
        
    public DateTime? MissionDate { get; init; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? ModsetName { get; init; }

    public string Owner { get; init; } = string.Empty;
        
    public SignupDto? Signups { get; init; }
}