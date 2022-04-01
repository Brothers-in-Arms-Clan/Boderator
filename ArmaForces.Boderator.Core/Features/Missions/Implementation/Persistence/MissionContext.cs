using ArmaForces.Boderator.Core.Missions.Models;
using Microsoft.EntityFrameworkCore;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence;

internal sealed class MissionContext : DbContext
{
    public MissionContext(DbContextOptions<MissionContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
        
    public DbSet<Mission> Missions { get; set; }
    public DbSet<Signups> Signups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>()
            .HasKey(x => new {x.SignupsId, x.Name});
    }
}