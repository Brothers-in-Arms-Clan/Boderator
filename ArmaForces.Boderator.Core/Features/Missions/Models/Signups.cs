using System;
using System.Collections.Generic;

namespace ArmaForces.Boderator.Core.Missions.Models;

public record Signups
{
    public int SignupsId { get; init; }
    
    public SignupStatus SignupStatus { get; init; }
        
    public DateTime StartDate { get; init; }
        
    public DateTime CloseDate { get; init; }
        
    public int MissionId { get; init; }

    public IReadOnlyList<Team> Teams { get; init; } = new List<Team>();
}