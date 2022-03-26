using System.Collections.Generic;

namespace ArmaForces.Boderator.BotService.Features.Signups.DTOs;

public record SlotDto
{
    public long? SlotId { get; init; }
        
    public string Name { get; init; } = string.Empty;

    public List<object> RequiredDlcs { get; init; } = new();

    public string Vehicle { get; init; } = string.Empty;
    
    public string? Occupant { get; init; }
}
