using System;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

public record MissionDto
{
    public long MissionId { get; init; }

    public string Title { get; init; } = string.Empty;
        
    public DateTime? MissionDate { get; init; }
        
    public SignupDto? Signups { get; init; }
}