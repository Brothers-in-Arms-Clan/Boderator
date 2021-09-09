using System;
using ArmaForces.Boderator.BotService.Discord;
using ArmaForces.Boderator.BotService.Documentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ArmaForces.Boderator.BotService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private OpenApiInfo OpenApiConfiguration { get; } = new()
        {
            Title = "ArmaForces Boderator API",
            Description = "Does nothing.",
            Version = "v3",
            Contact = new OpenApiContact
            {
                Name = "ArmaForces",
                Url = new Uri("https://armaforces.com")
            }
        };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddControllers();
            services.AddDocumentation(OpenApiConfiguration);

            services.AddDiscordService(Helpers.Configuration.DiscordToken);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.AddDocumentation(OpenApiConfiguration);
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "api/{controller}/{action}");
            });
        }
    }
}
