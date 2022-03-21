using ArmaForces.Boderator.Core.Missions.Persistence;
using ArmaForces.Boderator.Core.Signups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.DependencyInjection
{
    public static class BoderatorCoreServiceExtensions
    {
        public static IServiceCollection AddBoderatorCore(this IServiceCollection services, string connectionString)
            => services
                .AddDbContext<MissionContext>(options => options.UseSqlite(connectionString))
                .AddDbContext<SignupsContext>(options => options.UseSqlite(connectionString))
                .AutoAddInterfacesAsScoped(typeof(BoderatorCoreServiceExtensions).Assembly);
    }
}
