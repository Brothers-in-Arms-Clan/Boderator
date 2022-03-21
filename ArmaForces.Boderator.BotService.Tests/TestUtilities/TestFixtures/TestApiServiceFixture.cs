using System;
using System.Net.Http;
using ArmaForces.Boderator.BotService.Configuration;
using ArmaForces.Boderator.Core.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ArmaForces.Boderator.BotService.Tests.TestUtilities.TestFixtures
{
    public class TestApiServiceFixture : IDisposable
    {
        private const int Port = 43421;
        private readonly SocketsHttpHandler _socketsHttpHandler;
        private readonly IHost _host;

        public HttpClient HttpClient { get; }

        public TestApiServiceFixture()
        {
            _socketsHttpHandler = new SocketsHttpHandler();
            HttpClient = new HttpClient(_socketsHttpHandler)
            {
                BaseAddress = new Uri($"http://localhost:{Port}")
            };

            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(ConfigureWebBuilder())
                .Build();

            _host.Start();
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            _socketsHttpHandler.Dispose();
            _host.Dispose();
            GC.SuppressFinalize(this);
        }

        private static Action<IWebHostBuilder> ConfigureWebBuilder() => webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.ConfigureServices(
                x => x.AddOrReplaceSingleton(new TestConfigurationFactory().CreateConfiguration()));
            webBuilder.UseKestrel(x => x.ListenLocalhost(Port));
        };
    }
}
