using System;
using System.IO;
using ArmaForces.Boderator.Core.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.Tests.TestUtilities;

public class DatabaseTestBase : IDisposable
{
    private static readonly string TestConnectionString = "Data Source=" + Path.Join(Directory.GetCurrentDirectory(), "test.db");

    protected readonly IServiceProvider ServiceProvider = new ServiceCollection()
        .AddBoderatorCore(TestConnectionString)
        .BuildServiceProvider();

    protected IDbContextTransaction? DbContextTransaction { get; init; }

    protected DatabaseTestBase() { }

    public void Dispose() => DbContextTransaction?.Dispose();
}
