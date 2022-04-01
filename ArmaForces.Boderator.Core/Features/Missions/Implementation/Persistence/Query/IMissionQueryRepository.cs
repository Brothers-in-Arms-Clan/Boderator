using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Query;

internal interface IMissionQueryRepository
{
    Task<Mission?> GetMission(int missionId);
        
    Task<List<Mission>> GetMissions();
}
