using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions.Helpers;

internal class SignupsDbHelper
{
    private readonly MissionsDbHelper _missionsDbHelper;
    private readonly MissionContext _missionContext;

    public SignupsDbHelper(
        MissionsDbHelper missionsDbHelper,
        MissionContext missionContext)
    {
        _missionsDbHelper = missionsDbHelper;
        _missionContext = missionContext;
    }

    public async Task<Signups> CreateTestSignups(long missionId)
    {
        var signup = SignupsFixture.CreateTestSignup(missionId);

        var addedEntry = _missionContext.Signups.Add(signup);
        await _missionContext.SaveChangesAsync();

        return addedEntry.Entity;
    }

    public async Task<Signups> CreateTestSignups()
    {
        var mission = await _missionsDbHelper.CreateTestMission();
        return await CreateTestSignups(mission.MissionId);
    }
}