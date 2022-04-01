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

public class MissionQueryServiceIntegrationTests : DatabaseTestBase
{
    private readonly MissionsDbHelper _missionsDbHelper;
    private readonly IMissionQueryService _missionQueryService;

    public MissionQueryServiceIntegrationTests()
    {
        _missionsDbHelper = ServiceProvider.GetRequiredService<MissionsDbHelper>();
        _missionQueryService = ServiceProvider.GetRequiredService<IMissionQueryService>();

        var missionContext = ServiceProvider.GetRequiredService<MissionContext>();
        DbContextTransaction = missionContext.Database.BeginTransaction();
    }
    
    [Fact, Trait("Category", "Integration")]
    public async Task GetMissions_NoMissionsInDatabase_ReturnsEmptyList()
    {
        var result = await _missionQueryService.GetMissions();
        result.ShouldBeSuccess(new List<Mission>());
    }
    
    [Fact, Trait("Category", "Integration")]
    public async Task GetMissions_OneMissionInDatabase_ReturnsOneMission()
    {
        var createdMission = await _missionsDbHelper.CreateTestMission();

        var result = await _missionQueryService.GetMissions();
        
        result.ShouldBeSuccess(new List<Mission>{createdMission});
    }

    [Fact, Trait("Category", "Integration")]
    public async Task GetMission_MissionIdInDatabase_ReturnsMission()
    {
        var createdMission = await _missionsDbHelper.CreateTestMission();

        var result = await _missionQueryService.GetMission(createdMission.MissionId);
        
        result.ShouldBeSuccess(createdMission);
    }

    [Fact, Trait("Category", "Integration")]
    public async Task GetMission_MissionIdNotInDatabase_ReturnsFailure()
    {
        var createdMission = await _missionsDbHelper.CreateTestMission();
        var notExistingMissionId = createdMission.MissionId + 1;

        var result = await _missionQueryService.GetMission(notExistingMissionId);

        result.ShouldBeFailure($"Mission with ID {notExistingMissionId} does not exist.");
    }
}