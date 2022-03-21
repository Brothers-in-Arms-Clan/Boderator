using System.Linq;
using ArmaForces.Boderator.Core.DependencyInjection;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArmaForces.Boderator.Core.Tests.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void AutoAddInterfacesAsScoped_EmptyCollection_RegistersOnlyInterfacesWithOneImplementation()
        {
            var serviceProvider = new ServiceCollection()
                .AutoAddInterfacesAsScoped(typeof(ServiceCollectionExtensionsTests).Assembly)
                .BuildServiceProvider();

            using (new AssertionScope())
            {
                serviceProvider.GetService<ITest1>().Should().BeNull();
                serviceProvider.GetService<ITest2>().Should().NotBeNull();
                serviceProvider.GetService<ITest3>().Should().BeNull();
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void AutoAddInterfacesAsScoped_ImplementationRegisteredAsSingleton_DoesNotReplaceExistingRegistration()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<ITest2, Test2>()
                .AutoAddInterfacesAsScoped(typeof(ServiceCollectionExtensionsTests).Assembly);

            AssertServiceRegisteredCorrectly<ITest2, Test2>(serviceCollection, ServiceLifetime.Singleton);
        }

        private static void AssertServiceRegisteredCorrectly<TService, TExpectedImplementation>(IServiceCollection serviceCollection, ServiceLifetime expectedLifetime)
        {
            var expectedServiceDescriptor = new ServiceDescriptor(typeof(TService), typeof(TExpectedImplementation), expectedLifetime);
            
            serviceCollection.Should()
                .ContainSingle(descriptor => descriptor.ServiceType == typeof(TService))
                .Which.Should()
                .BeEquivalentTo(expectedServiceDescriptor, $"the service {nameof(TService)} was registered as {expectedLifetime} and should not be replaced");
        }

        private interface ITest1 { }

        private interface ITest2 { }

        private interface ITest3 { }

        private class Test1 : ITest1 { }

        private class Test2 : ITest1, ITest2 { }
    }
}
