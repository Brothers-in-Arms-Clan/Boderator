using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArmaforcesMissionBot.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController
    {
        private readonly DiscordSocketClient _discordClient;

        public HealthController(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        [HttpGet]
        public IActionResult IndexAction()
        {
            return _discordClient.ConnectionState == ConnectionState.Connected
                ? new OkResult()
                : new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }
}