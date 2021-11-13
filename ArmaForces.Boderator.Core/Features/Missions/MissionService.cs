using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using ArmaForces.Boderator.Core.Missions.Persistence;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions
{
    internal class MissionService : IMissionService
    {
        private readonly IMissionRepository _missionRepository;

        public MissionService(IMissionRepository missionRepository)
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

        public async Task<Result<Mission>> GetMission(int missionId)
            => await _missionRepository.GetMission(missionId)
               ?? Result.Failure<Mission>("Mission with given ID does not exist.");

        public async Task<Result<List<Mission>>> GetMissions()
            => await _missionRepository.GetMissions();
    }
}
