using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using ArmaForces.Boderator.BotService.Features.Missions.Mappers;
using ArmaForces.Boderator.Core.Missions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions;

/// <summary>
/// Allows signups retrieval and creation.
/// </summary>
[Route("api/[controller]")]
public class SignupsController : ControllerBase
{
    private readonly ISignupsQueryService _signupsQueryService;

    /// <inheritdoc />
    public SignupsController(ISignupsQueryService signupsQueryService)
    {
        _signupsQueryService = signupsQueryService;
    }

    /// <remarks>
    /// Creates requested signups for a mission.
    /// Either "MissionId" of existing mission without signups or a "Mission" object which satisfies the preconditions for creating signups must be provided. 
    /// </remarks>
    [HttpPost(Name = "Create Signups")]
    [SwaggerResponse(StatusCodes.Status201Created, "Signups created")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    public ActionResult<long> CreateSignups([FromBody] SignupsCreateRequestDto request)
        => throw new NotImplementedException();

    /// <remarks>Updates signups with given <paramref name="signupsId"/>.</remarks>
    /// <param name="signupsId">Id of signups to update</param>
    [HttpPatch("{signupsId:long}", Name = "Update Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups updated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to update signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult UpdateSignups(long signupsId)
        => throw new NotImplementedException();

    /// <remarks>Deletes signups with given <paramref name="signupsId"/>.</remarks>
    /// <param name="signupsId">Id of signups to delete.</param>
    [HttpDelete("{signupsId:long}", Name = "Delete Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups deleted")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to delete signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult DeleteSignups(long signupsId)
        => throw new NotImplementedException();

    /// <remarks>Retrieves signups with given <paramref name="signupsId"/>.</remarks>
    /// <param name="signupsId">Id of signups to retrieve.</param>
    [HttpGet("{signupsId:long}", Name = "Get Signups")]
    [SwaggerResponse(StatusCodes.Status200OK, "Signups retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public async Task<ActionResult<SignupsDto>> GetSignups(long signupsId)
        => await _signupsQueryService.GetSignups(signupsId)
            .Map(SignupsMapper.Map)
            .Match<ActionResult<SignupsDto>, SignupsDto>(
                onSuccess: signups => Ok(signups),
                onFailure: error => BadRequest(error));

    /// <remarks>Returns all signups satisfying query parameters.</remarks>
    [HttpGet(Name = "Lookup Signups")]
    [SwaggerResponse(StatusCodes.Status200OK, "Signups retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult<List<SignupsDto>> LookupSignups()
        => throw new NotImplementedException();

    /// <remarks>Signs up a player for a given slot.</remarks>
    /// <param name="signupsId">Id of signups to sign-up player</param>
    /// <param name="request">Sign-up details</param>
    [HttpPost("{signupsId:long}/sign", Name = "Sign-up Player")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Player signed up")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to sign up player")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult SignUpPlayer(long signupsId, [FromBody] PlayerSignupRequestDto request)
        => throw new NotImplementedException();

    /// <remarks>Immediately opens given signups allowing all players to sign-up.</remarks>
    /// <param name="signupsId">Id of signups to open</param>
    [HttpPost("{signupsId:long}/open", Name = "Open Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups opened")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to open signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult OpenSignups(long signupsId)
        => throw new NotImplementedException();
        
    /// <remarks>Immediately closes given signups.</remarks>
    /// <param name="signupsId">Id of signups to close</param>
    [HttpPost("{signupsId:long}/close", Name = "Close Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups closed")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to close signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult CloseSignups(long signupsId)
        => throw new NotImplementedException();
}