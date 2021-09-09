using ArmaForces.Boderator.BotService.Features.Test;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArmaForces.Boderator.Tests
{
    public class BasicTest : TestBase
    {
        private readonly IMediator _mediator;
        public BasicTest()
        {
            _mediator = Provider.GetService<IMediator>();
        }

        [Fact]
        public async void SendTestCommand_ReturnsExpectedString()
        {
            string ret = await _mediator.Send(new TestCommand());
            Assert.True(ret == "Hello World!");
        }
    }
}
