using System;
using System.Collections.Generic;
using System.Net.Mime;
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
[Produces(MediaTypeNames.Application.Json)]
public class SignupsController : ControllerBase
{
    private readonly ISignupsQueryService _signupsQueryService;

    /// <inheritdoc />
    public SignupsController(ISignupsQueryService signupsQueryService)
    {
        _signupsQueryService = signupsQueryService;
    }

    /// <summary>Create Signups</summary>
    /// <remarks>
    /// Creates signups for a mission. Mission can have only one signups attached.
    /// MissionId of an existing mission, which is valid for opening signups (has all details filled), must be provided.
    /// Alternatively, Mission can be created alongside Signups through "mission" property but it must still be valid for opening signups.
    /// </remarks>
    [HttpPost(Name = "CreateSignups")]
    [SwaggerResponse(StatusCodes.Status201Created, "Signups created")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    public ActionResult<long> CreateSignups([FromBody] SignupsCreateRequestDto request)
        => throw new NotImplementedException();

    /// <summary>Update Signups</summary>
    /// <remarks>Updates signups with given <paramref name="signupsId"/>.</remarks>
    /// <param name="signupsId">Id of signups to update</param>
    [HttpPatch("{signupsId:long}", Name = "UpdateSignups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups updated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to update signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult UpdateSignups(long signupsId)
        => throw new NotImplementedException();

    /// <summary>Delete Signups</summary>
    /// <remarks>Deletes signups with given <paramref name="signupsId"/>.</remarks>
    /// <param name="signupsId">Id of signups to delete.</param>
    [HttpDelete("{signupsId:long}", Name = "DeleteSignups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups deleted")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to delete signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult DeleteSignups(long signupsId)
        => throw new NotImplementedException();

    /// <summary>Get Signups</summary>
    /// <remarks>Retrieves signups with given <paramref name="signupsId"/>.</remarks>
    /// <param name="signupsId">Id of signups to retrieve.</param>
    [HttpGet("{signupsId:long}", Name = "GetSignups")]
    [SwaggerResponse(StatusCodes.Status200OK, "Signups retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public async Task<ActionResult<SignupsDto>> GetSignups(long signupsId)
        => await _signupsQueryService.GetSignups(signupsId)
            .Map(SignupsMapper.Map)
            .Match<ActionResult<SignupsDto>, SignupsDto>(
                onSuccess: signups => Ok(signups),
                onFailure: error => BadRequest(error));

    /// <summary>Lookup Signups</summary>
    /// <remarks>Returns all signups satisfying query parameters.</remarks>
    [HttpGet(Name = "LookupSignups")]
    [SwaggerResponse(StatusCodes.Status200OK, "Signups retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult<List<SignupsDto>> LookupSignups()
        => throw new NotImplementedException();

    /// <summary>Sign Up Player</summary>
    /// <remarks>Signs up a player for a given slot.</remarks>
    /// <param name="signupsId">Id of signups to sign-up player</param>
    /// <param name="request">Sign-up details</param>
    [HttpPost("{signupsId:long}/SignUp", Name = "SignUpPlayer")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Player signed up")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to sign up player")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult SignUpPlayer(long signupsId, [FromBody] PlayerSignUpRequestDto request)
        => throw new NotImplementedException();

    /// <summary>Sign Out Player</summary>
    /// <remarks>Signs out a player from a slot or whole signups.</remarks>
    /// <param name="signupsId">Id of signups to sign out player</param>
    /// <param name="request">Sign out details</param>
    [HttpPost("{signupsId:long}/SignOut", Name = "SignOutPlayer")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Player signed out")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to sign out player")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult SignOutPlayer(long signupsId, [FromBody] PlayerSignOutRequestDto request)
        => throw new NotImplementedException();

    /// <summary>Open Signups</summary>
    /// <remarks>Immediately opens given signups allowing all players to sign-up.</remarks>
    /// <param name="signupsId">Id of signups to open</param>
    [HttpPost("{signupsId:long}/Open", Name = "OpenSignups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups opened")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to open signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult OpenSignups(long signupsId)
        => throw new NotImplementedException();
        
    /// <summary>Close Signups</summary>
    /// <remarks>Immediately closes given signups.</remarks>
    /// <param name="signupsId">Id of signups to close</param>
    [HttpPost("{signupsId:long}/Close", Name = "CloseSignups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups closed")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to close signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult CloseSignups(long signupsId)
        => throw new NotImplementedException();
}