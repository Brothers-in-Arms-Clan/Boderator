using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.Core.Tests.Features.Missions.Helpers;

internal static class MissionsFixture
{
    public static Mission CreateTestMission()
    {
        return new Mission
        {
            Title = "Test mission",
            Owner = "Tester",
        };
    }
}
