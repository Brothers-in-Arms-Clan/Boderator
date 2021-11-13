using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions
{
    public interface IMissionService
    {
        Task<Result<Mission>> CreateMission(MissionCreateRequest missionCreateRequest);
        
        Task<Result<Mission>> GetMission(int missionId);

        Task<Result<List<Mission>>> GetMissions();
    }
}
