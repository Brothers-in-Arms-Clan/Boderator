using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Features.Health
{
    /// <summary>
    /// Allows obtaining application status.
    /// </summary>
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        /// <summary>
        /// Responds to a ping.
        /// </summary>
        [HttpGet("ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping() => Ok("pong");
    }
}
