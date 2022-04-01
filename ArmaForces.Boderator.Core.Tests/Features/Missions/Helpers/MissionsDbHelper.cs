using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions.Helpers;

internal class MissionsDbHelper
{
    private readonly MissionContext _missionContext;

    public MissionsDbHelper(MissionContext missionContext)
    {
        _missionContext = missionContext;
    }

    public async Task<Mission> CreateTestMission()
    {
        var mission = MissionsFixture.CreateTestMission();

        var addedEntry = _missionContext.Missions.Add(mission);
        await _missionContext.SaveChangesAsync();
        
        return addedEntry.Entity;
    }
}
