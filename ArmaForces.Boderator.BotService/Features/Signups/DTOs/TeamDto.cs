using System.Collections.Generic;

namespace ArmaForces.Boderator.BotService.Features.Signups.DTOs;

public record TeamDto
{
    public string Name { get; init; } = string.Empty;

    public List<object> RequiredDlcs { get; init; } = new();

    public string Vehicle { get; init; } = string.Empty;
    
    public List<SlotDto> Slots { get; init; } = new();
}