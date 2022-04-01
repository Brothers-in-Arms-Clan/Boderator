using System;
using System.Collections.Generic;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

public record SignupDto
{
    public int SignupId { get; init; }
    
    public SignupStatus SignupStatus { get; init; }
    
    public DateTime? StartDate { get; init; }
    
    public DateTime? CloseDate { get; init; }

    public List<TeamDto> Teams { get; init; } = new();
    
    public int MissionId { get; init; }
}