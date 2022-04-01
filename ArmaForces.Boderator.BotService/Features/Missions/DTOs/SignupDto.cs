using System;
using System.Collections.Generic;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

public record SignupDto
{
    public long SignupId { get; init; }
    
    public SignupsStatus Status { get; init; }
    
    public DateTime? StartDate { get; init; }
    
    public DateTime? CloseDate { get; init; }

    public List<TeamDto> Teams { get; init; } = new();
    
    public long MissionId { get; init; }
}