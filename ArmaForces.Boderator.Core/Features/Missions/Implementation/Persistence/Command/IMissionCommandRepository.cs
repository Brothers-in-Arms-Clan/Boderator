using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Command
{
    internal interface IMissionCommandRepository
    {
        Task<Result<Mission>> CreateMission(Mission missionToCreate);
    }
}
