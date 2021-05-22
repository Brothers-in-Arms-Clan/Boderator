using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using ArmaForces.Boderator.BotService.DataClasses;

namespace ArmaForces.Boderator.BotService.Controllers
{
    [Route("users/")]
    [ApiController]
    [Produces("application/json")]
    public class DLCController : ControllerBase
    {
        private readonly Users _users;

        public DLCController(Users users)
        {
            _users = users;
        }

        //Adds DLC to user
        [HttpPut("{userID}/dlcadd/{dlcName}")]
        public void AddDlcToUser(ulong userID, SingleUser.DLC dlcName)
        {
            if (!(_users.UsersList.Any(x => x.UserID == userID)))
            {
                Response.StatusCode = 404;
                Response.WriteAsync($"User not found");
                return;
            }    

            if (_users.UsersList.Where(x => x.UserID == userID).Any(x => x.DLCList.Contains(dlcName)))
            {
                Response.WriteAsync($"DLC already added");
                return;
            }

            _users.UsersList.Find(x => x.UserID == userID).DLCList.Add(dlcName);

            Response.WriteAsync($"DLC added");
            return;
        }

        //List user with given ID. "0" lists all users
        [HttpGet("{userID}")]
        public void UserOnServer(ulong userID)
        {
            JArray usersArray = new JArray();
            var usersOnServer = _users.UsersList;

            if (userID != 0)
            {
                usersOnServer = usersOnServer
                       .FindAll(x => x.UserID == userID);
            }

            if (usersOnServer.Count == 0)
            {
                Response.StatusCode = 404;
                Response.WriteAsync("User not found");
                return;
            }

            foreach (var user in usersOnServer)
            {
                var objUser = new JObject();
                objUser.Add("UserID", user.UserID);

                var dlcArray = new JArray();

                foreach (var dlc in user.DLCList)
                {
                    var objDLC = new JObject();
                    objDLC.Add("DLC", dlc.ToString());
                    dlcArray.Add(objDLC);
                }
                objUser.Add("DLC List", dlcArray);

                usersArray.Add(objUser);
            }

            Response.WriteAsync($"{usersArray.ToString()}");
        }

        //Finds users wiht given DLC
        [HttpGet("{dlcName}/users")]
        public void UsersWithDLC(String dlcName)
        {
            var usersArray = new JArray();
            var usersOnServer = _users.UsersList;

            SingleUser.DLC dlcClass;

            try 
            {
                dlcClass = (SingleUser.DLC)Enum.Parse(typeof(SingleUser.DLC), dlcName);
            }
            catch
            {
                Response.StatusCode = 404;
                Response.WriteAsync("DLC not found");
                return;
            }

            foreach (var user in usersOnServer)
            {
                if (user.DLCList.Contains(dlcClass))
                {
                    var objUser = new JObject();
                    objUser.Add("UserID", user.UserID);
                    usersArray.Add(objUser);
                }
            }

            Response.WriteAsync($"{usersArray.ToString()}");
        }

        //Check if user has all DLCs
        [HttpGet("DLC")]
        public IActionResult UserhasDLC(ulong userID, String dlcs)
        {
            if (dlcs == null)
            {
                return (IActionResult)NotFound("No given DLC");
            }

            List<SingleUser.DLC> dlcList = new List<SingleUser.DLC>();
            
            foreach (string singleDlc in dlcs.Split(','))
            {

                try
                {
                    var dlcClass = (SingleUser.DLC)Enum.Parse(typeof(SingleUser.DLC), singleDlc);
                    dlcList.Add(dlcClass);
                }
                catch
                {
                    return (IActionResult)BadRequest("Wrong DLC name");
                }

            }

            var user = _users.UsersList.SingleOrDefault(x => x.UserID == userID);
            
            if (user is null)
            {
                return  (IActionResult)NotFound("User not found");
            }

            var dlcFound = !dlcList.Except(user.DLCList).Any();

            return dlcFound
                ? Ok("User has all given DLCs")
                : (IActionResult)NotFound("User doesn't have all given DLCs");
        }
    }
}

