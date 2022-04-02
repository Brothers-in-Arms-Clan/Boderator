using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;
using ArmaForces.Boderator.Core.Tests.Features.Missions.Helpers;
using ArmaForces.Boderator.Core.Tests.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions.Implementation;

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

        var missionContext = ServiceProvider.GetRequiredService<MissionContext>();
        DbContextTransaction = missionContext.Database.BeginTransaction();
    }
    
    [Fact, Trait("Category", "Integration")]
    public async Task GetOpenSignups_NoSignupsInDatabase_ReturnsEmptyList()
    {
        var result = await _signupsQueryService.GetOpenSignups();
        result.ShouldBeSuccess(new List<Signups>());
    }
    
    [Fact, Trait("Category", "Integration")]
    public async Task GetOpenSignups_NoOpenSignupsInDatabase_ReturnsEmptyList()
    {
        var testMission = await _missionsDbHelper.CreateTestMission();
        var signup = await _signupsDbHelper.CreateTestSignups(testMission.MissionId);

        var result = await _signupsQueryService.GetOpenSignups();
        result.ShouldBeSuccess(new List<Signups>{signup});
    }

    [Fact, Trait("Category", "Integration")]
    public async Task GetSignups_SignupsWithGivenIdDoesntExist_ReturnsFailure()
    {
        const int nonExistingSignupsId = 0;
        var result = await _signupsQueryService.GetSignups(nonExistingSignupsId);
        
        result.ShouldBeFailure($"Signup with ID {nonExistingSignupsId} not found");
    }

    [Fact, Trait("Category", "Integration")]
    public async Task GetSignups_SignupsWithGivenIdExists_ReturnsSignups()
    {
        var signups = await _signupsDbHelper.CreateTestSignups();
        var result = await _signupsQueryService.GetSignups(signups.SignupsId);
        
        result.ShouldBeSuccess(signups);
    }
}
