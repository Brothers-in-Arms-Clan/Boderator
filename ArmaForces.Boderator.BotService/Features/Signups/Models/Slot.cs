namespace ArmaForces.Boderator.BotService.Features.Signups.Models
{
    public record Slot
    {
        public string Name { get; init; } = string.Empty;
        
        public string? Occupant { get; init; }
    }
}
