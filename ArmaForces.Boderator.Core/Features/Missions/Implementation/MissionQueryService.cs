using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions.Implementation;

internal class MissionQueryService : IMissionQueryService
{
    private readonly IMissionRepository _missionRepository;
    
    public MissionQueryService(IMissionRepository missionRepository)
    {
        _missionRepository = missionRepository;
    }
    
    public async Task<Result<Mission>> GetMission(int missionId)
        => await _missionRepository.GetMission(missionId)
           ?? Result.Failure<Mission>($"Mission with ID {missionId} does not exist.");

    public async Task<Result<List<Mission>>> GetMissions()
        => await _missionRepository.GetMissions();
}
