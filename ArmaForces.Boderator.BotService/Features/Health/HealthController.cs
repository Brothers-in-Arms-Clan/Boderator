using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Features.Health
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("pong");
    }
}
