using System.Collections.Generic;

namespace ArmaForces.Boderator.Core.Signups.Models
{
    public record Slot
    {
        public long? SlotId { get; init; } 
            
        public string Name { get; init; } = string.Empty;

        public List<object> RequiredDlcs { get; init; } = new();

        public string Vehicle { get; init; } = string.Empty;
        
        public string? Occupant { get; init; }
    }
}
