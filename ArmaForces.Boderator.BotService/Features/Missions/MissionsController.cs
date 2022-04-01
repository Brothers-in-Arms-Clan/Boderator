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
/// Allows missions data retrieval and creation.
/// </summary>
[Route("api/[controller]")]
public class MissionsController : Controller
{
    private readonly IMissionCommandService _missionCommandService;
    private readonly IMissionQueryService _missionQueryService;

    public MissionsController(IMissionCommandService missionCommandService, IMissionQueryService missionQueryService)
    {
        _missionCommandService = missionCommandService;
        _missionQueryService = missionQueryService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost(Name = "Create Mission")]
    [SwaggerResponse(StatusCodes.Status201Created, "The mission was created")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    public async Task<ActionResult<MissionDto>> CreateMission([FromBody] MissionCreateRequestDto request)
        => await _missionCommandService.CreateMission(MissionMapper.Map(request))
            .Map(MissionMapper.Map)
            .Match<ActionResult<MissionDto>, MissionDto>(
                onSuccess: mission => Created(mission.MissionId.ToString(), mission),
                onFailure: error => BadRequest(error));

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Updated mission data.</returns>
    [HttpPatch("{missionId:int}", Name = "Update Mission")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The mission was updated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to update the mission")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Mission not found")]
    public ActionResult<MissionDto> UpdateMission(int missionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Deleted mission data.</returns>
    [HttpDelete("{missionId:int}", Name = "Delete Mission")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The mission was deleted")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Not authorized to delete the mission")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Mission not found")]
    public ActionResult<MissionDto> DeleteMission(int missionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "Get Missions")]
    [SwaggerResponse(StatusCodes.Status200OK, "Missions retrieved")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Request is invalid")]
    public async Task<ActionResult<List<MissionDto>>> GetMissions()
        => await _missionQueryService.GetMissions()
            .Map(MissionMapper.Map)
            .Match<ActionResult<List<MissionDto>>, List<MissionDto>>(
                onSuccess: missions => Ok(missions),
                onFailure: error => BadRequest(error));

    /// <summary>
    /// Retrieves mission with given <paramref name="missionId"/>.
    /// </summary>
    /// <param name="missionId">Unique identifier of a mission</param>
    /// <returns></returns>
    [HttpGet("{missionId:int}", Name = "Get Mission")]
    [SwaggerResponse(StatusCodes.Status200OK, "Mission retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Mission not found")]
    public async Task<ActionResult<MissionDto>> GetMission(int missionId)
        => await _missionQueryService.GetMission(missionId)
            .Map(MissionMapper.Map)
            .Match<ActionResult<MissionDto>, MissionDto>(
                onSuccess: mission => Ok(mission),
                onFailure: error => NotFound(error));

    private ActionResult<T> ReturnSomething<T>(Result<T> result)
        => result.Match(
            onSuccess: x => Ok(x),
            onFailure: error => (ActionResult<T>) BadRequest(error));
}