using System;
using System.Collections.Generic;

namespace ArmaForces.Boderator.Core.Missions.Models;

public record Signups
{
    public long SignupsId { get; init; }

    public ushort Status { get; init; } = (ushort) SignupsStatus.Closed;
        
    public DateTime StartDate { get; init; }
        
    public DateTime CloseDate { get; init; }
        
    public long MissionId { get; init; }

    public IReadOnlyList<Team> Teams { get; init; } = new List<Team>();
}