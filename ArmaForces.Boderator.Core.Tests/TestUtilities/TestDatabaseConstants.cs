using System.IO;

namespace ArmaForces.Boderator.Core.Tests.TestUtilities;

public static class TestDatabaseConstants
{
    public static readonly string TestConnectionString = "Data Source=" + Path.Join(Directory.GetCurrentDirectory(), "test.db");
}
