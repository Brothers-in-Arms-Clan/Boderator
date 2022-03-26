using System;
using System.Collections.Generic;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using ArmaForces.Boderator.Core.Signups.Models;

namespace ArmaForces.Boderator.BotService.Features.Signups.DTOs;

public record SignupDto
{
    public int SignupId { get; init; }
    
    public SignupStatus SignupStatus { get; init; }
    
    public DateTime? StartDate { get; init; }
    
    public DateTime? CloseDate { get; init; }

    public MissionDto Mission { get; init; } = new();

    public List<TeamDto> Teams { get; init; } = new();
}