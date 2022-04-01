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
/// Allows signups data retrieval and creation.
/// </summary>
[Route("api/[controller]")]
public class SignupsController : ControllerBase
{
    private readonly ISignupsQueryService _signupsQueryService;

    public SignupsController(ISignupsQueryService signupsQueryService)
    {
        _signupsQueryService = signupsQueryService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "Create Signups")]
    [SwaggerResponse(StatusCodes.Status201Created, "Signups created")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    public ActionResult<long> CreateSignups([FromBody] SignupsCreateRequestDto request)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpPatch("{signupsId:long}", Name = "Update Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups updated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to update signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult UpdateSignups(long signupsId)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpDelete("{signupsId:long}", Name = "Delete Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups deleted")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to delete signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult DeleteSignups(long signupsId)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpGet("{signupsId:long}", Name = "Get Signups")]
    [SwaggerResponse(StatusCodes.Status200OK, "Signups retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public async Task<ActionResult<SignupsDto>> GetSignups(long signupsId)
        => await _signupsQueryService.GetSignups(signupsId)
            .Map(SignupsMapper.Map)
            .Match<ActionResult<SignupsDto>, SignupsDto>(
                onSuccess: signups => Ok(signups),
                onFailure: error => BadRequest(error));

    /// <summary>
    /// Returns all signups satisfying query parameters.
    /// </summary>
    /// <remarks>Lookup Signups</remarks>
    /// <returns></returns>
    [HttpGet(Name = "Lookup Signups")]
    [SwaggerResponse(StatusCodes.Status200OK, "Signups retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult<List<SignupsDto>> LookupSignups()
        => throw new NotImplementedException();

    /// <summary>
    /// Signs up a player for a given slot.
    /// </summary>
    [HttpPost("{signupsId:long}/sign", Name = "Sign Player")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Player signed up")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to sign up player")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult SignPlayer(long signupsId, [FromBody] PlayerSignupRequestDto request)
        => throw new NotImplementedException();

    /// <summary>
    /// Opens given signups allowing all players to sign-up.
    /// </summary>
    [HttpPost("{signupsId:long}/open", Name = "Open Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups opened")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to open signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult OpenSignups(long signupsId)
        => throw new NotImplementedException();
        
    /// <summary>
    /// Immediately closes given signups.
    /// </summary>
    [HttpPost("{signupsId:long}/close", Name = "Close Signups")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Signups closed")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to close signups")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Signups not found")]
    public ActionResult CloseSignups(long signupsId)
        => throw new NotImplementedException();
}