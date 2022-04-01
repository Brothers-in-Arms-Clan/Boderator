using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions.Helpers;

internal class SignupsDbHelper
{
    private readonly MissionContext _missionContext;

    public SignupsDbHelper(MissionContext missionContext)
    {
        _missionContext = missionContext;
    }

    public async Task<Signups> CreateTestSignup(int missionId)
    {
        var signup = SignupsFixture.CreateTestSignup(missionId);

        var addedEntry = _missionContext.Signups.Add(signup);
        await _missionContext.SaveChangesAsync();

        return addedEntry.Entity;
    }
}