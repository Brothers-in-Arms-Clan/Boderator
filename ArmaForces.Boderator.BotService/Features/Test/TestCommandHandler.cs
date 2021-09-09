using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ArmaForces.Boderator.BotService.Features.Test
{
    public class TestCommandHandler : IRequestHandler<TestCommand, string>
    {
        Task<string> IRequestHandler<TestCommand, string>.Handle(TestCommand request, CancellationToken cancellationToken) => 
            Task.Run(() => "Hello World!", cancellationToken);
    }
}
