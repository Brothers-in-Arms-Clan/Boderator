using System.Collections.Generic;
using ArmaForces.Boderator.Core.Dlcs.Models;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

public record TeamDto
{
    public string Name { get; init; } = string.Empty;

    public List<Dlc> RequiredDlcs { get; init; } = new();

    public string Vehicle { get; init; } = string.Empty;
    
    public List<SlotDto> Slots { get; init; } = new();
}