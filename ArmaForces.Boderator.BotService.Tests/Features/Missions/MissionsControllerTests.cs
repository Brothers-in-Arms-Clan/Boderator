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
        public async Task CreateMission_InvalidRequest_ReturnsBadRequest()
        {
            var missionCreateRequestWithoutOwner = new MissionCreateRequestDto
            {
                Title = Fixture.Create<string>(),
                Description = Fixture.Create<string>()
            };
            
            var result = await HttpPostAsync("api/missions", missionCreateRequestWithoutOwner);

            result.ShouldBeFailure("Bad Request");
        } 

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
        
        [Fact]
        public async Task GetMission_MissionExists_ReturnsExistingMission()
        {
            var missionCreateRequest = new MissionCreateRequestDto
            {
                Title = Fixture.Create<string>(),
                Owner = Fixture.Create<string>(),
                Description = Fixture.Create<string>()
            };
            
            var missionCreateResult = await HttpPostAsync<MissionCreateRequestDto, MissionDto>("api/missions", missionCreateRequest);

            var expectedMission = new MissionDto
            {
                Title = missionCreateResult.Value.Title,
                MissionDate = missionCreateResult.Value.MissionDate,
                MissionId = missionCreateResult.Value.MissionId
            };
            
            var result = await HttpGetAsync<MissionDto>($"api/missions/{expectedMission.MissionId}");

            result.ShouldBeSuccess(expectedMission);
        }
    }
}
