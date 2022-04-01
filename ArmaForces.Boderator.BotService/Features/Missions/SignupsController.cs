using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using ArmaForces.Boderator.BotService.Features.Missions.Mappers;
using ArmaForces.Boderator.Core.Missions;
using CSharpFunctionalExtensions;
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
    public ActionResult<long> CreateSignup([FromBody] SignupsCreateRequestDto request)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpPatch("{signupsId:long}", Name = "Update Signups")]
    public ActionResult UpdateSignup(long signupsId)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpDelete("{signupsId:long}", Name = "Delete Signups")]
    public ActionResult DeleteSignup(long signupsId)
        => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signupsId"></param>
    /// <returns></returns>
    [HttpGet("{signupsId:long}", Name = "Get Signups")]
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
    [HttpGet(Name = "Query Signups")]
    public ActionResult<List<SignupsDto>> QuerySignups()
        => throw new NotImplementedException();

    /// <summary>
    /// Signs up a player for a given slot.
    /// </summary>
    [HttpPost("{signupsId:long}/sign", Name = "Sign Player")]
    public ActionResult SignPlayer(long signupsId, [FromBody] PlayerSignupRequestDto request)
        => throw new NotImplementedException();

    [HttpPost("{signupsId:long}/open", Name = "Open Signups")]
    public ActionResult OpenSignups(long signupsId)
        => throw new NotImplementedException();
        
    /// <summary>
    /// Immediately closes given signups.
    /// </summary>
    [HttpPost("{signupsId:long}/close", Name = "Close Signups")]
    public ActionResult CloseSignups(long signupsId)
        => throw new NotImplementedException();
}