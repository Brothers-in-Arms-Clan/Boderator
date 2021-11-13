using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions.Persistence
{
    internal interface IMissionRepository
    {
        Task<Mission?> GetMission(int missionId);
        
        Task<List<Mission>> GetMissions();
        Task<Result<Mission>> CreateMission(Mission missionToCreate);
    }
}
