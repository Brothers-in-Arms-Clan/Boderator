using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions;
using ArmaForces.Boderator.Core.Missions.Implementation;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Query;
using ArmaForces.Boderator.Core.Missions.Models;
using ArmaForces.Boderator.Core.Tests.TestUtilities;
using AutoFixture;
using Moq;
using Xunit;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions;

public class MissionQueryServiceUnitTests
{
    private readonly Fixture _fixture = new();
    
    [Fact, Trait("Category", "Unit")]
    public async Task GetMissions_RepositoryEmpty_ReturnsEmptyList()
    {
        var missionQueryRepository = CreateRepositoryMock(new List<Mission>());
        var missionQueryService = new MissionQueryService(missionQueryRepository);

        var result = await missionQueryService.GetMissions();
        
        result.ShouldBeSuccess(new List<Mission>());
    }
    
    [Fact, Trait("Category", "Unit")]
    public async Task GetMissions_RepositoryNotEmpty_ReturnsExpectedMissions()
    {
        var missionsInRepository = _fixture.CreateMany<Mission>(5).ToList();
        var missionQueryRepository = CreateRepositoryMock(missionsInRepository);
        var missionQueryService = new MissionQueryService(missionQueryRepository);

        var result = await missionQueryService.GetMissions();
        
        result.ShouldBeSuccess(missionsInRepository);
    }

    private static IMissionQueryRepository CreateRepositoryMock(IEnumerable<Mission> missions)
    {
        var missionQueryRepositoryMock = new Mock<IMissionQueryRepository>();
        missionQueryRepositoryMock
            .Setup(x => x.GetMissions())
            .Returns(Task.FromResult(missions.ToList()));

        return missionQueryRepositoryMock.Object;
    }
}
