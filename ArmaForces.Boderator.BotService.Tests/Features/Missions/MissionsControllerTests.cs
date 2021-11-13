using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using ArmaForces.Boderator.BotService.Tests.TestUtilities.TestBases;
using ArmaForces.Boderator.BotService.Tests.TestUtilities.TestFixtures;
using ArmaForces.Boderator.Core.Tests.TestUtilities;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace ArmaForces.Boderator.BotService.Tests.Features.Missions
{
    [Trait("Category", "Integration")]
    public class MissionsControllerTests : ApiTestBase
    {
        public MissionsControllerTests(TestApiServiceFixture testApi)
            : base(testApi) { }

        [Fact]
        public async Task CreateMission_ValidRequest_MissionCreated()
        {
            var missionCreateRequest = new MissionCreateRequestDto
            {
                Title = Fixture.Create<string>(),
                Owner = Fixture.Create<string>(),
                Description = Fixture.Create<string>()
            };
            
            var result = await HttpPostAsync("api/missions", missionCreateRequest);

            result.ShouldBeSuccess();
        }
    }
}
