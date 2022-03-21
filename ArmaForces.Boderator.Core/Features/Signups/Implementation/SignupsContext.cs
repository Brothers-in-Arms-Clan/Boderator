using ArmaForces.Boderator.Core.Signups.Models;
using Microsoft.EntityFrameworkCore;

namespace ArmaForces.Boderator.Core.Signups.Implementation
{
    internal sealed class SignupsContext : DbContext
    {
        public SignupsContext(DbContextOptions<SignupsContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<Signup> Signups { get; set; }
    }
}
