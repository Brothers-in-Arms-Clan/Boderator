using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions;

public interface IMissionQueryService
{
    Task<Result<Mission>> GetMission(long missionId);

    Task<Result<List<Mission>>> GetMissions();
}
