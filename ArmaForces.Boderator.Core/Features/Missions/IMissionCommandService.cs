using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions;

public interface IMissionCommandService
{
    Task<Result<Mission>> CreateMission(MissionCreateRequest missionCreateRequest);
}