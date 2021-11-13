using System.Collections.Generic;

namespace ArmaForces.Boderator.Core.Signups.Models
{
    public record Team
    {
        public string Name { get; init; } = string.Empty;

        public IReadOnlyList<Slot> Slots { get; init; } = new List<Slot>();
    }
}
