using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions;
using ArmaForces.Boderator.Core.Tests.Features.Missions.Helpers;
using ArmaForces.Boderator.Core.Tests.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions;

public class SignupsQueryServiceIntegrationTests : DatabaseTestBase
{
    private readonly MissionsDbHelper _missionsDbHelper;
    private readonly SignupsDbHelper _signupsDbHelper;
    private readonly ISignupsQueryService _signupsQueryService;

    public SignupsQueryServiceIntegrationTests()
    {
        _missionsDbHelper = ServiceProvider.GetRequiredService<MissionsDbHelper>();
        _signupsDbHelper = ServiceProvider.GetRequiredService<SignupsDbHelper>();
        _signupsQueryService = ServiceProvider.GetRequiredService<ISignupsQueryService>();

        // DbContextTransaction = _missionContext.Database.BeginTransaction();
    }
    
    [Fact, Trait("Category", "Integration")]
    public async Task GetOpenSignups_NoSignupsInDatabase_ReturnsEmptyList()
    {
        var result = await _signupsQueryService.GetOpenSignups();
        result.ShouldBeSuccess(new List<Core.Missions.Models.Signups>());
    }
    
    [Fact, Trait("Category", "Integration")]
    public async Task GetOpenSignups_NoOpenSignupsInDatabase_ReturnsEmptyList()
    {
        var testMission = await _missionsDbHelper.CreateTestMission();
        var signup = await _signupsDbHelper.CreateTestSignup(testMission.MissionId);

        var result = await _signupsQueryService.GetOpenSignups();
        result.ShouldBeSuccess(new List<Core.Missions.Models.Signups>{signup});
    }
}
