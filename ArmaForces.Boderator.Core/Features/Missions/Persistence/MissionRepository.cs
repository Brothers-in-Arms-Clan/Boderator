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
            var ddd = await _context.Missions.AddAsync(missionToCreate);
            
            return ddd is not null
                ? ddd.Entity
                : Result.Failure<Mission>("Failure creating mission.");
        }
    }
}
