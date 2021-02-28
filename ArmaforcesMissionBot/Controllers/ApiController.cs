using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ArmaforcesMissionBot.Features.Signups.Missions;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArmaforcesMissionBot.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly DiscordSocketClient _client;
        private readonly Config _config;
        private readonly MissionsArchiveData _missionsArchiveData;
        private readonly SignupsData _signupsData;
        private readonly BanHelper _banHelper;
        private readonly SignupHelper _signupHelper;
        private readonly MiscHelper _miscHelper;

        public ApiController(
            MissionsArchiveData missionsArchiveData,
            SignupsData signupsData,
            DiscordSocketClient client,
            BanHelper banHelper,
            SignupHelper signupHelper,
            MiscHelper miscHelper)
        {
            _missionsArchiveData = missionsArchiveData;
            _signupsData = signupsData;
            _client = client;
            _banHelper = banHelper;
            _signupHelper = signupHelper;
            _miscHelper = miscHelper;
        }

        [HttpGet("currentMission")]
        public IActionResult CurrentMission()
        {
            var mission = _signupsData.Missions
                .Concat(_missionsArchiveData.ArchiveMissions.Cast<IMission>())
                .Where(mission => mission.Date < DateTime.Now && mission.Date.AddHours(3) > DateTime.Now)
                .OrderBy(mission => mission.Date)
                .FirstOrDefault();

            return mission is null
                ? (IActionResult) NotFound("No ongoing mission.")
                : Ok(mission);
        }

        [HttpGet("missions")]
        public void Missions(DateTime? fromDateTime = null, DateTime? toDateTime = null, bool includeArchive = false, uint ttl = 0)
        {
            fromDateTime = fromDateTime ?? DateTime.MinValue;
            toDateTime = toDateTime ?? DateTime.MaxValue;

            toDateTime = toDateTime ==
                         new DateTime(toDateTime.Value.Year, toDateTime.Value.Month, toDateTime.Value.Day, 0, 0, 0)
                ? new DateTime(toDateTime.Value.Year,
                    toDateTime.Value.Month,
                    toDateTime.Value.Day,
                    23,
                    59,
                    59)
                : toDateTime;

            JArray missionArray = new JArray();
            var openMissionsEnumerable = missions.Missions
                .Where(x => x.Editing == Features.Signups.Missions.Mission.EditEnum.NotEditing)
                .Where(x => x.Date >= fromDateTime)
                .Where(x => x.Date <= toDateTime)
                .Reverse();
            foreach (var mission in openMissionsEnumerable)
            {
                var objMission = new JObject();
                objMission.Add("title", mission.Title);
                objMission.Add("date", mission.Date.ToString("yyyy-MM-ddTHH:mm:ss"));
                objMission.Add("closeDate", mission.CloseTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                objMission.Add("image", mission.Attachment);
                objMission.Add("description", mission.Description);
                objMission.Add("modlist", mission.Modlist);
                objMission.Add("modlistName", mission.ModlistName);
                objMission.Add("modlistUrl", mission.ModlistUrl);
                objMission.Add("id", mission.SignupChannel);
                objMission.Add("freeSlots", Helpers.MiscHelper.CountFreeSlots(mission));
                objMission.Add("allSlots", Helpers.MiscHelper.CountAllSlots(mission));
                objMission.Add("state", "Open");

                missionArray.Add(objMission);
            }

            if(includeArchive)
            {
                var archiveMissionsEnumerable = _missionsArchiveData.ArchiveMissions.AsEnumerable()
                    .Where(x => x.Date >= fromDateTime)
                    .Where(x => x.Date <= toDateTime)
                    .Reverse();
                foreach (var mission in archiveMissionsEnumerable)
                {
                    var objMission = new JObject();
                    objMission.Add("title", mission.Title);
                    objMission.Add("date", mission.Date.ToString("yyyy-MM-ddTHH:mm:ss"));
                    objMission.Add("closeDate", mission.CloseTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                    objMission.Add("image", mission.Attachment);
                    objMission.Add("description", mission.Description);
                    objMission.Add("modlist", mission.Modlist);
                    objMission.Add("modlistName", mission.ModlistName);
                    objMission.Add("modlistUrl", mission.ModlistUrl);
                    objMission.Add("archive", true);
                    objMission.Add("freeSlots", mission.FreeSlots);
                    objMission.Add("allSlots", mission.AllSlots);
                    objMission.Add("state", mission.Date < DateTime.Now ? "Archived" : "Closed");

                    missionArray.Add(objMission);
                }
            }

            Response.ContentType = "application/json; charset=utf-8";
            if (ttl != 0)
            {
                Response.Headers.Add("Cache-Control", $"public, max-age={ttl}");
            }

            Response.WriteAsync($"{missionArray.ToString()}");
        }

        [HttpGet("mission")]
        public void Mission(ulong id, ulong userID)
        {
            if (!_banHelper.IsUserSpamBanned(userID) && _signupHelper.ShowMissionToUser(userID, id))
            {
                var mission = _signupsData.Missions.Single(x => x.SignupChannel == id);

                var serialized = JsonConvert.SerializeObject(mission);
                Response.WriteAsync($"{serialized}");
            }
            else
            {
                Response.StatusCode = 503;
                Response.WriteAsync("Banned");
            }
        }

        [HttpGet("signup")]
        public async Task Signup(ulong missionID, ulong teamID, ulong userID, string slotID)
        {
            _signupsData.BanAccess.Wait(-1);
            try
            {
                if (_signupsData.SignupBans.ContainsKey(userID) ||
                    _signupsData.SpamBans.ContainsKey(userID))
                {
                    Response.StatusCode = 503;
                    await Response.WriteAsync("Banned");
                    return;
                }
            }
            finally
            {
                _signupsData.BanAccess.Release();
            }

            if (_signupsData.Missions.Any(x => x.SignupChannel == missionID))
            {
                var mission = _signupsData.Missions.Single(x => x.SignupChannel == missionID);

                mission.Access.Wait(-1);
                try
                {
                    if (!mission.SignedUsers.Contains(userID))
                    {
                        if (mission.Teams.Any(x => x.TeamMsg == teamID))
                        {
                            var team = mission.Teams.Single(x => x.TeamMsg == teamID);

                            if (team.Slots.Any(x => x.Emoji.Name == slotID && x.Count > x.Signed.Count()))
                            {
                                var channel = _client.GetGuild(_config.AFGuild).GetTextChannel(missionID);
                                var teamMsg = await channel.GetMessageAsync(teamID) as IUserMessage;

                                var embed = teamMsg.Embeds.Single();

                                if (!mission.SignedUsers.Contains(userID))
                                {
                                    var slot = team.Slots.Single(x => x.Emoji.Name == slotID);
                                    slot.Signed.Add(userID);
                                    mission.SignedUsers.Add(userID);

                                    var newDescription = _miscHelper.BuildTeamSlots(team);

                                    var newEmbed = new EmbedBuilder
                                    {
                                        Title = embed.Title,
                                        Color = embed.Color
                                    };

                                    if (newDescription.Count == 2)
                                        newEmbed.WithDescription(newDescription[0] + newDescription[1]);
                                    else if (newDescription.Count == 1)
                                        newEmbed.WithDescription(newDescription[0]);

                                    if (embed.Footer.HasValue)
                                        newEmbed.WithFooter(embed.Footer.Value.Text);

                                    await teamMsg.ModifyAsync(x => x.Embed = newEmbed.Build());
                                    await Response.WriteAsync("Success");
                                    return;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    mission.Access.Release();
                }
            }

            Response.StatusCode = 400;
            await Response.WriteAsync("Data invalid");
        }

        [HttpGet("signoff")]
        public async Task Signoff(ulong missionID, ulong teamID, ulong userID, string slotID)
        {
            _signupsData.BanAccess.Wait(-1);
            try
            {
                if (_signupsData.SignupBans.ContainsKey(userID) ||
                    _signupsData.SpamBans.ContainsKey(userID))
                {
                    Response.StatusCode = 503;
                    await Response.WriteAsync("Banned");
                    return;
                }
            }
            finally
            {
                _signupsData.BanAccess.Release();
            }

            if (_signupsData.Missions.Any(x => x.SignupChannel == missionID))
            {
                var mission = _signupsData.Missions.Single(x => x.SignupChannel == missionID);

                mission.Access.Wait(-1);
                try
                {
                    if (mission.SignedUsers.Contains(userID))
                    {
                        if (mission.Teams.Any(x => x.TeamMsg == teamID))
                        {
                            var team = mission.Teams.Single(x => x.TeamMsg == teamID);

                            if (team.Slots.Any(x => x.Emoji.Name == slotID))
                            {
                                var channel = _client.GetGuild(_config.AFGuild).GetTextChannel(missionID);
                                var teamMsg = await channel.GetMessageAsync(teamID) as IUserMessage;

                                var embed = teamMsg.Embeds.Single();

                                if (mission.SignedUsers.Contains(userID))
                                {
                                    var slot = team.Slots.Single(x => x.Emoji.Name == slotID);
                                    slot.Signed.Remove(userID);
                                    mission.SignedUsers.Remove(userID);

                                    var newDescription = _miscHelper.BuildTeamSlots(team);

                                    var newEmbed = new EmbedBuilder
                                    {
                                        Title = embed.Title,
                                        Color = embed.Color
                                    };

                                    if (newDescription.Count == 2)
                                        newEmbed.WithDescription(newDescription[0] + newDescription[1]);
                                    else if (newDescription.Count == 1)
                                        newEmbed.WithDescription(newDescription[0]);

                                    if (embed.Footer.HasValue)
                                        newEmbed.WithFooter(embed.Footer.Value.Text);

                                    await teamMsg.ModifyAsync(x => x.Embed = newEmbed.Build());
                                    await Response.WriteAsync("Success");
                                    return;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    mission.Access.Release();
                }
            }

            Response.StatusCode = 400;
            await Response.WriteAsync("Data invalid");
        }

        [HttpGet("emotes")]
        public void Emotes()
        {
            var emotes = _client.GetGuild(_config.AFGuild).Emotes;
            JArray emotesArray = new JArray();
            foreach (var emote in emotes)
            {
                var emoteObj = new JObject();
                var animated = emote.Animated ? "a" : "";
                emoteObj.Add("id", $"<{animated}:{emote.Name}:{emote.Id}>");
                emoteObj.Add("url", emote.Url);

                emotesArray.Add(emoteObj);
            }
            Response.WriteAsync($"{emotesArray.ToString()}");
        }

        [HttpGet("users")]
        public void Users()
        {
            var guild = _client.GetGuild(_config.AFGuild);
            var users = guild.Users;
            var makerRole = guild.GetRole(_config.MissionMakerRole);
            JArray usersArray = new JArray();
            foreach (var user in users)
            {
                var userObj = new JObject();
                userObj.Add("id", user.Id);
                userObj.Add("name", user.Username);
                userObj.Add("isMissionMaker", user.Roles.Contains(makerRole));

                usersArray.Add(userObj);
            }
            Response.WriteAsync($"{usersArray.ToString()}");
        }

        [HttpPost("createMission")]
        public async Task CreateMissionAsync(Mission mission)
        {
            Console.WriteLine(JsonConvert.SerializeObject(mission));

            mission.Editing = Features.Signups.Missions.Mission.EditEnum.New;
            signups.Missions.Add(mission);

            if (Helpers.SignupHelper.CheckMissionComplete(mission))
            {
                var guild = _client.GetGuild(_config.AFGuild);

                var signupChannel = await _signupHelper.CreateChannelForMission(guild, mission, _signupsData);
                mission.SignupChannel = signupChannel.Id;

                await _signupHelper.CreateMissionMessagesOnChannel(guild, mission, signupChannel);
            }
            else
            {
                await Response.WriteAsync($"Incorrect data");
            }
        }
    }
}
