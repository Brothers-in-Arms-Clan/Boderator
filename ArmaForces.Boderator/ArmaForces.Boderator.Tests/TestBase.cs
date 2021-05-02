using System;
using ArmaForces.Boderator.BotService;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaForces.Boderator.Tests
{
    public class TestBase
    {
        protected IServiceProvider Provider { get; }

        public TestBase()
        {
            ServiceCollection sc = new();
            sc.AddMediatR(typeof(TestBase).Assembly, typeof(Startup).Assembly);
            Provider = sc.BuildServiceProvider();
        }
    }
}
