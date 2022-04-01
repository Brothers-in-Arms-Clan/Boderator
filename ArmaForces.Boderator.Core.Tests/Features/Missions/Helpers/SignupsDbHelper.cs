using System;
using System.Collections.Generic;
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

    public async Task<Core.Missions.Models.Signups> CreateTestSignup(int missionId)
    {
        var signup = SignupsFixture.CreateTestSignup(missionId);

        var addedEntry = _missionContext.Signups.Add(signup);
        await _missionContext.SaveChangesAsync();

        return addedEntry.Entity;
    }
}

internal static class SignupsFixture
{
    public static Core.Missions.Models.Signups CreateTestSignup(int missionId)
    {
        return new Core.Missions.Models.Signups
        {
            MissionId = missionId,
            SignupStatus = SignupStatus.Open,
            StartDate = DateTime.Now,
            CloseDate = DateTime.Now.AddHours(1),
            Teams = new List<Team>()
        };
    }
}
