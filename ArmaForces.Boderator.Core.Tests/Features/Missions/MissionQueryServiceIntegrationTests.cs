using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Models;
using ArmaForces.Boderator.Core.Tests.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions;

public class MissionQueryServiceIntegrationTests : DatabaseTestBase
{
    private readonly MissionContext _missionContext;
    private readonly IMissionQueryService _missionQueryService;
    
    public MissionQueryServiceIntegrationTests()
    {
        _missionContext = ServiceProvider.GetRequiredService<MissionContext>();
        _missionQueryService = ServiceProvider.GetRequiredService<IMissionQueryService>();

        DbContextTransaction = _missionContext.Database.BeginTransaction();
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
        var mission = CreateTestMission();

        var addedEntry = _missionContext.Missions.Add(mission);
        await _missionContext.SaveChangesAsync();

        var result = await _missionQueryService.GetMissions();
        
        result.ShouldBeSuccess(new List<Mission>{addedEntry.Entity});
    }

    [Fact, Trait("Category", "Integration")]
    public async Task GetMission_MissionIdInDatabase_ReturnsMission()
    {
        var mission = CreateTestMission();

        var createdMission = _missionContext.Missions.Add(mission).Entity;
        await _missionContext.SaveChangesAsync();

        var result = await _missionQueryService.GetMission(createdMission.MissionId);
        
        result.ShouldBeSuccess(createdMission);
    }

    [Fact, Trait("Category", "Integration")]
    public async Task GetMission_MissionIdNotInDatabase_ReturnsFailure()
    {
        var mission = CreateTestMission();

        var createdMission = _missionContext.Missions.Add(mission).Entity;
        await _missionContext.SaveChangesAsync();
        var notExistingMissionId = createdMission.MissionId + 1;

        var result = await _missionQueryService.GetMission(notExistingMissionId);

        result.ShouldBeFailure($"Mission with ID {notExistingMissionId} does not exist.");
    }

    private static Mission CreateTestMission()
    {
        return new Mission
        {
            Title = "Test mission",
            Owner = "Tester",
        };
    }
}