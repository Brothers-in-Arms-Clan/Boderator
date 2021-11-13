using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all interfaces from <paramref name="assembly"/> with single implementation type as scoped service.
        /// Does not replace existing registrations.
        /// </summary>
        /// <param name="services">Service collection where services will be registered</param>
        /// <param name="assembly">Assembly which will be searched for interfaces and implementations.</param>
        /// <returns>Service collection for method chaining.</returns>
        public static IServiceCollection AutoAddInterfacesAsScoped(this IServiceCollection services, Assembly assembly)
        {
            assembly.DefinedTypes
                .Where(x => x.ImplementedInterfaces.Any())
                .SelectMany(
                    implementingClass => implementingClass.ImplementedInterfaces,
                    (implementingClass, implementedInterface) => new {implementedInterface, implementingClass})
                .GroupBy(x => x.implementedInterface)
                .Where(x => x.Count() == 1)
                .Select(x => x.Single())
                .Where(x => services.IsNotServiceRegistered(x.implementedInterface))
                .ToList()
                .ForEach(x => services.AddScoped(x.implementedInterface, x.implementingClass));

            return services;
        }

        private static bool IsNotServiceRegistered(this IServiceCollection services, Type serviceType)
            => !IsServiceRegistered(services, serviceType);

        private static bool IsServiceRegistered(this IServiceCollection services, Type serviceType)
            => services.Any(descriptor => descriptor.ServiceType == serviceType);
    }
}
