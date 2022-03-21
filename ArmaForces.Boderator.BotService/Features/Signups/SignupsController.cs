using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Features.Signups.DTOs;
using ArmaForces.Boderator.Core.Signups;
using ArmaForces.Boderator.Core.Signups.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Features.Signups
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
        public ActionResult CreateSignup([FromBody] SignupCreateRequestDto request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpPatch("{signupId:int}", Name = "Update Signup")]
        public ActionResult ModifySignup(int signupId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpDelete("{signupId:int}", Name = "Delete Signup")]
        public ActionResult DeleteSignup(int signupId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpGet("{signupId:int}", Name = "Get Signup")]
        public async Task<ActionResult<Signup>> GetSignup(int signupId)
            => await _signupsQueryService.GetSignup(signupId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "Get Signups")]
        public ActionResult<List<Signup>> GetSignups()
        {
            throw new NotImplementedException();
        }
    }
}
