using System.Collections.Generic;

namespace ArmaForces.Boderator.BotService.Features.Signups.Models
{
    public record Team
    {
        public string Name { get; init; } = string.Empty;

        public IReadOnlyList<Slot> Slots { get; init; } = new List<Slot>();
    }
}
