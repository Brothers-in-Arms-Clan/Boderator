using System.Threading;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Discord;
using ArmaForces.Boderator.BotService.Features.Test;
using Discord;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Controllers
{
    [Route("api/[controller]")]
    public class DebugController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IDiscordService _discordService;

        public DebugController(IMediator mediator, IDiscordService discordService)
        {
            _mediator = mediator;
            _discordService = discordService;
        }

        [HttpGet("Test")]
        public async Task<ActionResult> Test(CancellationToken cToken)
        {
            string response = await _mediator.Send(new TestCommand(), cToken);
            return Ok(response);
        }

        [HttpGet("GetDiscordStatus")]
        public ActionResult GetDiscordStatus() => Ok(_discordService.GetDiscordClientStatus());

        [HttpPut("SetBotStatus/{status}")]
        public async Task<ActionResult> SetBotStatus(string status, ActivityType type = ActivityType.Playing)
        {
            await _discordService.SetBotStatus(status, type);
            return Ok();
        }
    }
}
