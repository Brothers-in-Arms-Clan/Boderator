namespace ArmaForces.Boderator.BotService.Features.Signups.DTOs;

public record SlotDto
{
    public string Name { get; init; } = string.Empty;
    
    public string? Occupant { get; init; }
}
