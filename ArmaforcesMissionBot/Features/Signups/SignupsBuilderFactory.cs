using System;
using ArmaforcesMissionBot.Features.Signups.Missions.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace ArmaforcesMissionBot.Features.Signups
{
    public class SignupsBuilderFactory : ISignupsBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SignupsBuilderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISignupsBuilder CreateSignupsBuilder()
        {
            return new SignupsBuilder(_serviceProvider.GetService<IMissionValidator>());
        }
    }

    public interface ISignupsBuilderFactory
    {
        ISignupsBuilder CreateSignupsBuilder();
    }
}
