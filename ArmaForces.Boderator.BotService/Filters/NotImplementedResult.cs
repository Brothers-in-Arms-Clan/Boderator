using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ArmaForces.Boderator.BotService.Filters;

/// <summary>
/// A <see cref="StatusCodeResult"/> that when
/// executed will produce a Not Implemented (501) response.
/// </summary>
[DefaultStatusCode(DefaultStatusCode)]
public class NotImplementedResult : StatusCodeResult
{
    private const int DefaultStatusCode = StatusCodes.Status501NotImplemented;

    /// <summary>
    /// Creates a new <see cref="NotImplementedResult"/> instance.
    /// </summary>
    public NotImplementedResult()
        : base(DefaultStatusCode)
    {
    }
}
