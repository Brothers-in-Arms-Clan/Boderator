using ArmaForces.Boderator.Core.Missions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.DependencyInjection
{
    public static class BoderatorCoreServiceExtensions
    {
        public static IServiceCollection AddBoderatorCore(this IServiceCollection services)
            => services
                .AddDbContext<MissionContext>(options => options.UseSqlite())
                .AutoAddInterfacesAsScoped(typeof(BoderatorCoreServiceExtensions).Assembly);
    }
}
