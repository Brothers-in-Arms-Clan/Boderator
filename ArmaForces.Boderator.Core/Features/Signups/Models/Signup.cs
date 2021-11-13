using System;
using System.Collections.Generic;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.Core.Signups.Models
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
