namespace ArmaForces.Boderator.Core.Missions.Models
{
    public record Slot
    {
        public long? SlotId { get; init; } 
            
        public string Name { get; init; } = string.Empty;

        public string Vehicle { get; init; } = string.Empty;
        
        public string? Occupant { get; init; }
    }
}
