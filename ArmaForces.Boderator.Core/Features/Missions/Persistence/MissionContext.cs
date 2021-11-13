using ArmaForces.Boderator.Core.Missions.Models;
using Microsoft.EntityFrameworkCore;

namespace ArmaForces.Boderator.Core.Missions.Persistence
{
    internal class MissionContext : DbContext
    {
        public MissionContext(DbContextOptions<MissionContext> options)
            : base(options) { }
        
        public DbSet<Mission> Missions { get; set; } 
    }
}
