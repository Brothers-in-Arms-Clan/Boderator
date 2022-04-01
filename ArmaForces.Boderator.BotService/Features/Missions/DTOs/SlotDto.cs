using System.Collections.Generic;
using ArmaForces.Boderator.Core.Dlcs.Models;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

public record SlotDto
{
    public long? SlotId { get; init; }
        
    public string Name { get; init; } = string.Empty;

    public List<Dlc> RequiredDlcs { get; init; } = new();

    public string Vehicle { get; init; } = string.Empty;
    
    public string? Occupant { get; init; }
}
