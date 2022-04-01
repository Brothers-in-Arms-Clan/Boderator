using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using ArmaForces.Boderator.BotService.Tests.TestUtilities.TestBases;
using ArmaForces.Boderator.BotService.Tests.TestUtilities.TestFixtures;
using ArmaForces.Boderator.Core.Tests.TestUtilities;
using AutoFixture;
using Xunit;

namespace ArmaForces.Boderator.BotService.Tests.Features.Missions
{
    [Trait("Category", "Integration")]
    public class MissionsControllerTests : ApiTestBase
    {
        public MissionsControllerTests(TestApiServiceFixture testApi)
            : base(testApi) { }
        
        [Theory, ClassData(typeof(CreateMissionInvalidRequestTestData)), Trait("Category", "Integration")]
        public async Task CreateMission_InvalidRequest_ReturnsBadRequest(MissionCreateRequestDto missionCreateRequestDto)
        {
            var result = await HttpPostAsync<MissionCreateRequestDto, MissionDto>("api/missions", missionCreateRequestDto);

            result.ShouldBeFailure();
        }

        [Theory, ClassData(typeof(CreateMissionValidRequestTestData)), Trait("Category", "Integration")]
        public async Task CreateMission_ValidRequest_MissionCreated(MissionCreateRequestDto missionCreateRequestDto)
        {
            var result = await HttpPostAsync<MissionCreateRequestDto, MissionDto>("api/missions", missionCreateRequestDto);
            
            result.ShouldBeSuccess(missionCreateRequestDto);
        }
        
        [Fact, Trait("Category", "Integration")]
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
                Description = missionCreateRequest.Description,
                Owner = missionCreateRequest.Owner,
                MissionDate = missionCreateResult.Value.MissionDate,
                MissionId = missionCreateResult.Value.MissionId
            };
            
            var result = await HttpGetAsync<MissionDto>($"api/missions/{expectedMission.MissionId}");

            result.ShouldBeSuccess(expectedMission);
        }
    }

    public class CreateMissionValidRequestTestData : TheoryData<MissionCreateRequestDto>
    {
        private readonly MissionCreateRequestDto _minimalRequest = new()
        {
            Title = "Test mission title",
            Owner = "Test mission owner"
        };
        
        public CreateMissionValidRequestTestData()
        {
            var testCases = new List<MissionCreateRequestDto>
            {
                _minimalRequest,
                _minimalRequest with
                {
                    Description = "Test mission description"
                },
                _minimalRequest with
                {
                    MissionDate = DateTime.Now.AddHours(-1)
                },
                _minimalRequest with
                {
                    ModsetName = "Test-mission-modset"
                },
                _minimalRequest with
                {
                    Description = "Test mission description",
                    MissionDate = DateTime.Now.AddHours(-1),
                    ModsetName = "Test-mission-modset"
                }
            };

            foreach (var testCase in testCases) Add(testCase);
        }
    }
    
    public class CreateMissionInvalidRequestTestData : TheoryData<MissionCreateRequestDto>
    {
        public CreateMissionInvalidRequestTestData()
        {
            var testCases = new List<MissionCreateRequestDto>
            {
                new()
                {
                    Title = "Test mission title without owner"
                },
                new()
                {
                    Owner = "Test mission owner without title"
                },
                new()
                {
                    Title = "Test mission title",
                    Owner = "Test mission owner",
                    ModsetName = "Modset name with whitespace characters"
                }
            };

            foreach (var testCase in testCases) Add(testCase);
        }
    }
}
