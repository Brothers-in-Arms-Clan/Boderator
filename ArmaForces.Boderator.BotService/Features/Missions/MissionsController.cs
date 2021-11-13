using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using ArmaForces.Boderator.BotService.Features.Missions.Mappers;
using ArmaForces.Boderator.Core.Missions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Features.Missions
{
    /// <summary>
    /// Allows missions data retrieval and creation.
    /// </summary>
    [Route("api/[controller]")]
    public class MissionsController : Controller
    {
        private readonly IMissionService _missionService;
        
        public MissionsController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<MissionDto>> CreateMission([FromBody] MissionCreateRequestDto request)
            => await _missionService.CreateMission(MissionMapper.Map(request))
                .Map(MissionMapper.Map)
                .Match<ActionResult<MissionDto>, MissionDto>(
                    onSuccess: mission => Created(mission.MissionId.ToString(), mission),
                    onFailure: error => BadRequest(error));

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Updated mission data.</returns>
        [HttpPatch("{missionId:int}")]
        public ActionResult<MissionDto> UpdateMission(int missionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Deleted mission data.</returns>
        [HttpDelete("{missionId:int}")]
        public ActionResult<MissionDto> DeleteMission(int missionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<MissionDto>>> GetMissions()
            => await _missionService.GetMissions()
                .Map(MissionMapper.Map)
                .Match<ActionResult<List<MissionDto>>, List<MissionDto>>(
                    onSuccess: missions => Ok(missions),
                    onFailure: error => BadRequest(error));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="missionId"></param>
        /// <returns></returns>
        [HttpGet("{missionId:int}")]
        public async Task<ActionResult<MissionDto>> GetMission(int missionId)
            => await _missionService.GetMission(missionId)
                .Map(MissionMapper.Map)
                .Match<ActionResult<MissionDto>, MissionDto>(
                    onSuccess: mission => Ok(mission),
                    onFailure: error => NotFound(error));

        private ActionResult<T> ReturnSomething<T>(Result<T> result)
            => result.Match(
                onSuccess: x => Ok(x),
                onFailure: error => (ActionResult<T>) BadRequest(error));
    }
}
