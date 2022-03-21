using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions.Implementation;

internal class MissionCommandService : IMissionCommandService
{
    private readonly IMissionCommandRepository _missionCommandRepository;

    public MissionCommandService(IMissionCommandRepository missionCommandRepository)
    {
        _missionCommandRepository = missionCommandRepository;
    }

    public async Task<Result<Mission>> CreateMission(MissionCreateRequest missionCreateRequest)
        => await _missionCommandRepository.CreateMission(
            new Mission
            {
                Title = missionCreateRequest.Title,
                Owner = missionCreateRequest.Owner,
                MissionTime = missionCreateRequest.MissionTime,
                ModsetName = missionCreateRequest.ModsetName
            });
}