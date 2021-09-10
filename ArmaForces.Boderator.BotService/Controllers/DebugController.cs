using System.Threading;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Discord;
using Discord;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Controllers
{
    [Route("api/[controller]")]
    public class DebugController : Controller
    {
        private readonly IDiscordService _discordService;

        public DebugController(IDiscordService discordService)
        {
            _discordService = discordService;
        }

        [HttpGet("Test")]
        public async Task<ActionResult> Test(CancellationToken cToken)
        {
            return Ok();
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
