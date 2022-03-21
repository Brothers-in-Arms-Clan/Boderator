using System;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Signups.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.DependencyInjection
{
    public static class BoderatorCoreServiceExtensions
    {
        public static IServiceCollection AddBoderatorCore(this IServiceCollection services,
            Func<IServiceProvider, string> connectionStringFactory)
            => services
                .AddDbContext<MissionContext>(connectionStringFactory)
                .AddDbContext<SignupsContext>(connectionStringFactory)
                .AutoAddInterfacesAsScoped(typeof(BoderatorCoreServiceExtensions).Assembly);

        private static IServiceCollection AddDbContext<T>(this IServiceCollection services,
            Func<IServiceProvider, string> connectionStringFactory)
            where T : DbContext
            => services.AddDbContext<T>((serviceProvider, options) =>
                options.UseSqlite(connectionStringFactory(serviceProvider)));
    }
}
