using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using Microsoft.EntityFrameworkCore;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Query;

internal class MissionQueryRepository : IMissionQueryRepository
{
    private readonly MissionContext _context;
        
    public MissionQueryRepository(MissionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Mission?> GetMission(long missionId)
        => await _context.Missions.FindAsync(missionId);

    public async Task<List<Mission>> GetMissions()
        => await _context.Missions.ToListAsync();
}
