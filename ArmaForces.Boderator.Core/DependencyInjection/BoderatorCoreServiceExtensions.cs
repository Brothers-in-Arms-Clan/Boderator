using ArmaForces.Boderator.Core.Missions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.DependencyInjection
{
    public static class BoderatorCoreServiceExtensions
    {
        public static IServiceCollection AddBoderatorCore(this IServiceCollection services, string connectionString)
            => services
                .AddDbContext<MissionContext>(options => options.UseSqlServer(connectionString))
                .AutoAddInterfacesAsScoped(typeof(BoderatorCoreServiceExtensions).Assembly);
    }
}
