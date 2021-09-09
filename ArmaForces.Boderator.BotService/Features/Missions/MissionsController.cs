using System;
using System.Collections.Generic;
using ArmaForces.Boderator.BotService.Features.Missions.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Features.Missions
{
    public class MissionsController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<MissionDto> CreateMission([FromBody] MissionCreateRequestDto request)
        {
            throw new NotImplementedException();
        }

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
        public ActionResult<List<MissionDto>> GetMissions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="missionId"></param>
        /// <returns></returns>
        [HttpGet("{missionId:int}")]
        public ActionResult<MissionDto> GetMission(int missionId)
        {
            throw new NotImplementedException();
        }
    }
}
