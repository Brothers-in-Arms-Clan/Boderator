using System;

namespace ArmaForces.Boderator.Core.Missions.Models;

public record Mission
{
    public long MissionId { get; init; }

    public string Title { get; init; } = string.Empty;
    
    public string? Description { get; init; } = string.Empty;

    public DateTime? MissionDate { get; init; }

    public string? ModsetName { get; init; }

    public string Owner { get; init; } = string.Empty;
        
    public Signups? Signups { get; init; }
}