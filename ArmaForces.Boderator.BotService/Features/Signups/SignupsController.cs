using System;
using System.Collections.Generic;
using ArmaForces.Boderator.BotService.Features.Signups.DTOs;
using ArmaForces.Boderator.Core.Signups.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArmaForces.Boderator.BotService.Features.Signups
{
    public class SignupsController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSignup([FromBody] SignupCreateRequestDto request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpPatch("{signupId:int}")]
        public ActionResult ModifySignup(int signupId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpDelete("{signupId:int}")]
        public ActionResult DeleteSignup(int signupId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signupId"></param>
        /// <returns></returns>
        [HttpGet("{signupId:int}")]
        public ActionResult<Signup> GetSignup(int signupId)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<Signup>> GetSignups()
        {
            throw new NotImplementedException();
        }
    }
}
