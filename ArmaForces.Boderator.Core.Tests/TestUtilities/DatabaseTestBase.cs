using System;
using ArmaForces.Boderator.Core.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.Tests.TestUtilities;

public class DatabaseTestBase : IDisposable
{
    protected readonly IServiceProvider ServiceProvider = new ServiceCollection()
        .AddBoderatorCore(_ => TestDatabaseConstants.TestConnectionString)
        .BuildServiceProvider();

    protected IDbContextTransaction? DbContextTransaction { get; init; }

    protected DatabaseTestBase() { }

    public void Dispose() => DbContextTransaction?.Dispose();
}
