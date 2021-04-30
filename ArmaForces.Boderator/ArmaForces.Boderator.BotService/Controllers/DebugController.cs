using System.Threading;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Test;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Controllers
{
    [Route("api/[controller]")]
    public class DebugController : Controller
    {
        private readonly IMediator _mediator;

        public DebugController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Test(CancellationToken cToken)
        {
            string response = await _mediator.Send(new TestCommand(), cToken);
            return Ok(response);
        }
    }
}
