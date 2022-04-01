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
        [HttpPost(Name = "Create Signup")]
        public ActionResult<long> CreateSignup([FromBody] SignupCreateRequestDto request)
            => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpPatch("{signupId:long}", Name = "Update Signup")]
        public ActionResult UpdateSignup(long signupId)
            => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpDelete("{signupId:long}", Name = "Delete Signup")]
        public ActionResult DeleteSignup(long signupId)
            => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpGet("{signupId:long}", Name = "Get Signup")]
        public async Task<ActionResult<SignupDto>> GetSignup(long signupId)
            => await _signupsQueryService.GetSignup(signupId)
                .Map(SignupsMapper.Map)
                .Match<ActionResult<SignupDto>, SignupDto>(
                    onSuccess: signup => Ok(signup),
                    onFailure: error => BadRequest(error));

        /// <summary>
        /// Returns all signups satisfying query parameters.
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "Get Signups")]
        public ActionResult<List<Core.Missions.Models.Signups>> GetSignups()
            => throw new NotImplementedException();

        /// <summary>
        /// Signs up a player for a given slot.
        /// </summary>
        [HttpPost("{signupId:long}/sign", Name = "Sign Player")]
        public ActionResult SignPlayer(long signupId, [FromBody] PlayerSignupRequestDto request)
            => throw new NotImplementedException();

        [HttpPost("{signupId:long}/open", Name = "Open Signup")]
        public ActionResult OpenSignup(long signupId)
            => throw new NotImplementedException();
        
        /// <summary>
        /// Immediately closes given signups.
        /// </summary>
        [HttpPost("{signupId:long}/close", Name = "Close Signup")]
        public ActionResult CloseSignup(long signupId)
            => throw new NotImplementedException();
    }
}
