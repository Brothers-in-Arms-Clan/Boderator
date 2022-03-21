using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Tests.TestUtilities.TestBases;
using ArmaForces.Boderator.BotService.Tests.TestUtilities.TestFixtures;
using ArmaForces.Boderator.Core.Tests.TestUtilities;
using Xunit;

namespace ArmaForces.Boderator.BotService.Tests.Features.Health
{
    [Trait("Category", "Integration")]
    public class HealthControllerTests : ApiTestBase
    {
        public HealthControllerTests(TestApiServiceFixture testApi)
            : base(testApi) { }

        [Fact]
        public async Task Ping_AllOk_ReturnsPong()
        {
            var result = await HttpGetAsync("api/health/ping");

            result.ShouldBeSuccess("pong");
        }
    }
}
