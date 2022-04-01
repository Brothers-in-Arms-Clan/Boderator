using System;
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

        [Fact, Trait("Category", "Unit")]
        public void AutoAddInterfacesAsScoped_EmptyCollection_RegistersOnlyInterfacesFromThisAssembly()
        {
            var serviceCollection = new ServiceCollection()
                .AutoAddInterfacesAsScoped(typeof(ServiceCollectionExtensionsTests).Assembly);
            
            using (new AssertionScope())
            {
                serviceCollection.Should()
                    .OnlyContain(x => x.ServiceType.Assembly == typeof(ServiceCollectionExtensionsTests).Assembly);
            }
        }

        private static void AssertServiceRegisteredCorrectly<TService, TExpectedImplementation>(IServiceCollection serviceCollection, ServiceLifetime expectedLifetime)
        {
            var expectedServiceDescriptor = new ServiceDescriptor(typeof(TService), typeof(TExpectedImplementation), expectedLifetime);
            
            serviceCollection.Should()
                .ContainSingle(descriptor => descriptor.ServiceType == typeof(TService))
                .Which.Should()
                .BeEquivalentTo(expectedServiceDescriptor, $"the service {nameof(TService)} was registered as {expectedLifetime} and should not be replaced");
        }
        
        /// <summary>
        /// Used to check if interfaces from other assemblies aren't registered automatically when there is one implementation.
        /// </summary>
        private enum TestEnum { }

        private interface ITest1 { }

        private interface ITest2 { }

        private interface ITest3 { }

        private class Test1 : ITest1 { }

        private class Test2 : ITest1, ITest2 { }
        
        /// <summary>
        /// Used to check if interfaces from other assemblies aren't registered automatically when there is one implementation.
        /// </summary>
        private class Test3 : ICloneable2
        {
            public object Clone() => throw new NotImplementedException();
        }
    }
}
