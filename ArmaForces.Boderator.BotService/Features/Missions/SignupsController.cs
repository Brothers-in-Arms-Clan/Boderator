using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using ArmaForces.Boderator.BotService.Features.Missions.Mappers;
using ArmaForces.Boderator.Core.Missions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<long> CreateSignups([FromBody] SignupsCreateRequestDto request)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpPatch("{signupsId:long}", Name = "Update Signups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult UpdateSignups(long signupsId)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpDelete("{signupsId:long}", Name = "Delete Signups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeleteSignups(long signupsId)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpGet("{signupsId:long}", Name = "Get Signups")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SignupsDto>> GetSignups(long signupsId)
        => await _signupsQueryService.GetSignups(signupsId)
            .Map(SignupsMapper.Map)
            .Match<ActionResult<SignupsDto>, SignupsDto>(
                onSuccess: signups => Ok(signups),
                onFailure: error => BadRequest(error));

    /// <summary>
    /// Returns all signups satisfying query parameters.
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "Lookup Signups")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<SignupsDto>> LookupSignups()
        => throw new NotImplementedException();

    /// <summary>
    /// Signs up a player for a given slot.
    /// </summary>
    [HttpPost("{signupsId:long}/sign", Name = "Sign Player")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult SignPlayer(long signupsId, [FromBody] PlayerSignupRequestDto request)
        => throw new NotImplementedException();

    [HttpPost("{signupsId:long}/open", Name = "Open Signups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult OpenSignups(long signupsId)
        => throw new NotImplementedException();
        
    /// <summary>
    /// Immediately closes given signups.
    /// </summary>
    [HttpPost("{signupsId:long}/close", Name = "Close Signups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult CloseSignups(long signupsId)
        => throw new NotImplementedException();
}