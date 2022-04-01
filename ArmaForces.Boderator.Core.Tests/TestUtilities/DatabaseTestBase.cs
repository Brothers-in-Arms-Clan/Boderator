using System;
using ArmaForces.Boderator.Core.DependencyInjection;
using ArmaForces.Boderator.Core.Tests.Features.Missions.Helpers;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.Tests.TestUtilities;

public class DatabaseTestBase : IDisposable
{
    protected readonly IServiceProvider ServiceProvider = new ServiceCollection()
        .AddBoderatorCore(_ => TestDatabaseConstants.TestConnectionString)
        .AddScoped<MissionsDbHelper>()
        .AddScoped<SignupsDbHelper>()
        .BuildServiceProvider();

    protected IDbContextTransaction? DbContextTransaction { get; init; }

    protected DatabaseTestBase() { }

    public void Dispose() => DbContextTransaction?.Dispose();
}
