using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions.Implementation;

internal class MissionCommandService : IMissionCommandService
{
    private readonly IMissionRepository _missionRepository;

    public MissionCommandService(IMissionRepository missionRepository)
    {
        _missionRepository = missionRepository;
    }

    public async Task<Result<Mission>> CreateMission(MissionCreateRequest missionCreateRequest)
        => await _missionRepository.CreateMission(
            new Mission
            {
                Title = missionCreateRequest.Title,
                Owner = missionCreateRequest.Owner,
                MissionTime = missionCreateRequest.MissionTime,
                ModsetName = missionCreateRequest.ModsetName
            });
}