using System;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Command;

internal class MissionCommandRepository : IMissionCommandRepository
{
    private readonly MissionContext _context;
        
    public MissionCommandRepository(MissionContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Result<Mission>> CreateMission(Mission missionToCreate)
    {
        var missionEntityEntry = await _context.Missions.AddAsync(missionToCreate);

        if (missionEntityEntry is null) return Result.Failure<Mission>("Failure creating mission.");
            
        await _context.SaveChangesAsync();
        return missionEntityEntry.Entity;
    }
}