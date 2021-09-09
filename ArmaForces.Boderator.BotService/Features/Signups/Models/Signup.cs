using System;
using System.Collections.Generic;
using ArmaForces.Boderator.BotService.Features.Missions.Models;

namespace ArmaForces.Boderator.BotService.Features.Signups.Models
{
    public record Signup
    {
        public int SignupsId { get; init; }
        
        public DateTime StartDate { get; init; }
        
        public DateTime CloseDate { get; init; }

        public Mission Mission { get; init; } = new Mission();

        public IReadOnlyList<Team> Teams { get; init; } = new List<Team>();
    }
}
