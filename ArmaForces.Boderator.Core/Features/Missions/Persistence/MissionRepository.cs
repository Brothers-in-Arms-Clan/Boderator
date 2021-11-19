using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ArmaForces.Boderator.Core.Missions.Persistence
{
    internal class MissionRepository : IMissionRepository
    {
        private readonly MissionContext _context;
        
        public MissionRepository(MissionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Mission?> GetMission(int missionId)
            => await _context.Missions.FindAsync(missionId);

        public async Task<List<Mission>> GetMissions()
            => await _context.Missions.ToListAsync();

        public async Task<Result<Mission>> CreateMission(Mission missionToCreate)
        {
            var missionEntityEntry = await _context.Missions.AddAsync(missionToCreate);

            if (missionEntityEntry is null) return Result.Failure<Mission>("Failure creating mission.");
            
            await _context.SaveChangesAsync();
            return missionEntityEntry.Entity;
        }
    }
}
