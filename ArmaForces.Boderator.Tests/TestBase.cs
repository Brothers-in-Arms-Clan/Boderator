using System;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Tests
{
    public class TestBase
    {
        protected IServiceProvider Provider { get; }

        public TestBase()
        {
            Provider = new ServiceCollection()
                .BuildServiceProvider();
        }
    }
}
