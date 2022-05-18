using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Exceptions;
using ArmaforcesMissionBot.Extensions;
using ArmaforcesMissionBot.Features;
using ArmaforcesMissionBot.Features.Modsets;
using ArmaforcesMissionBot.Features.Modsets.Constants;
using ArmaforcesMissionBot.Features.Signups.Importer;
using ArmaforcesMissionBot.Helpers;
using ArmaforcesMissionBotSharedClasses;
using CSharpFunctionalExtensions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Modules
{
    [Name("Sign-ups")]
    public class Signups : ModuleBase<SocketCommandContext>, IModule
    {
        public IServiceProvider _map { get; set; }
        public DiscordSocketClient _client { get; set; }
        public Config _config { get; set; }
        public OpenedDialogs _dialogs { get; set; }
        public CommandService _commands { get; set; }
        public IModsetProvider ModsetProvider { get; set; }
        public SignupsData SignupsData { get; set; }
        public SignupHelper SignupHelper { get; set; }
        public MiscHelper _miscHelper { get; set; }

        [Command("import-signups")]
        [Summary("Imports the records from the attached * .txt file or from a message (preferring a txt file if both are avaliable)." +
                 "Reads a file / message line by line, appending lines without the 'BIA!' prefix  to the previous command " +
                 "and then execute the commands in sequence. " +
                 "Ignores lines starting with '#' and '//', allowing comments.")]
        [ContextDMOrChannel]
        public async Task ImportSignups([Remainder]string missionContent = null) {
            if (_client.GetGuild(_config.AFGuild)
                .GetUser(Context.User.Id)
                .Roles.All(x => x.Id != _config.MissionMakerRole))
                await ReplyWithException<NotAuthorizedException>("You don't have the permission to create a mission.");

            if (SignupsData.Missions.Any(
                x =>
                    (x.Editing == Mission.EditEnum.New ||
                     x.Editing == Mission.EditEnum.Started) &&
                    x.Owner == Context.User.Id))
                await ReplyWithException<MissionEditionInProgressException>(
                    "You're creating/editing a mission already!");


            if (Context.Message.Attachments.Any(x => x.Filename.Contains(".txt"))) {
                using var client = new HttpClient();
                var response = await client.GetAsync(Context.Message.Attachments.First().Url);
                missionContent = await response.Content.ReadAsStringAsync();
            }

            if (missionContent is null)
                await ReplyWithException<InvalidCommandParametersException>("Incorrect command parameters.");

            var signupImporter = new SignupImporter(Context, _commands, _map, this);

            await signupImporter.ProcessMessage(missionContent);
            
            await ReplyAsync("Define the rest of the mission.");
        }

        [Command("new-mission")]
        [Summary("Creates a new mission, takes a name as a parameter.")]
        [ContextDMOrChannel]
        public async Task StartSignups([Remainder]string title)
        {
            var signups = _map.GetService<SignupsData>();

            if (SignupsData.GetCurrentlyEditedMission(Context.User.Id) != null)
                await ReplyAsync("Finish defining previous sing-ups first.");
            else
            {
                if (_client.GetGuild(_config.AFGuild).GetUser(Context.User.Id).Roles.Any(x => x.Id == _config.MissionMakerRole))
                {
                    var mission = new Mission();

                    mission.Title = title;
                    mission.Owner = Context.User.Id;
                    mission.Date = DateTime.Now;
                    mission.Editing = Mission.EditEnum.New;

                    SignupsData.Missions.Add(mission);

                    await ReplyAsync("Define rest of the mission.");
                }
                else
                    await ReplyAsync("You don't have the permission to create a mission.");
            }
        }

        [Command("briefing")]
        [Summary("Mission description definition, by adding a picture you add a picture to the mission.")]
        [ContextDMOrChannel]
        public async Task Description([Remainder]string description)
        {
            var mission = SignupsData.GetCurrentlyEditedMission(Context.User.Id);

            if (mission != null)
            {
                mission.Description = description;

                if (Context.Message.Attachments.Count > 0)
                {
                    mission.Attachment = Context.Message.Attachments.ElementAt(0).Url;
                }

                await ReplyAsync("Now attach a message with modlist from the modlist-for-mission channel.");
            }
            else
            {
                await ReplyAsync("First define name mission you moron.");
            }
        }

        [Command("modlist")]
        [Summary("Modlist link.")]
        [ContextDMOrChannel]
        public async Task Modlist([Remainder]string modsetNameOrUrl)
        {
            var mission = SignupsData.GetCurrentlyEditedMission(Context.User.Id);

            if (mission != null)
            {

                mission.ModlistUrl = mission.Modlist = modsetNameOrUrl;
                /*var modsetName = ModsetProvider.GetModsetNameFromUrl(modsetNameOrUrl);
                await ModsetProvider.GetModsetDownloadUrl(modsetName).Match(
                        onSuccess: url =>
                        {
                            mission.ModlistUrl = mission.Modlist = url.Replace(" ", "%20");
                            mission.ModlistName = modsetName;
                            return ReplyAsync($"Modset {modsetName} was found under {mission.ModlistUrl}.");
                        },
                        onFailure: error => ReplyAsync(error));*/

                await ReplyAsync("Now enter the date of the mission.");
            }
            else
            {
                await ReplyAsync("First define name mission you moron.");
            }
        }

        [Command("date")]
        [Summary("Define the mission start date in the format YYYY-MM-DD HH:MM.")]
        [ContextDMOrChannel]
        public async Task Date([Remainder] DateTime date) {
            if (date.IsInPast())
                await ReplyAsync(":warning: Given date is in the past!");
            else if (date.IsNoLaterThanDays(1)) await ReplyAsync(":warning: Given date is within less than 24 hours!");

            var mission = SignupsData.GetCurrentlyEditedMission(Context.User.Id);

            if (mission is null) {
                await ReplyAsync(":warning: You are not creating any mission.");
                return;
            }

            mission.Date = date;
            if (!mission.CustomClose)
                mission.CloseTime = date.AddMinutes(-60);

            await ReplyAsync($"Mission date set to {date}, in {date.FromNow()}.");
        }

        [Command("signups-date")]
        [Summary("Define the time when signups should close, such as the date in the format YYYY-MM-DD HH:MM.")]
        [ContextDMOrChannel]
        public async Task Close([Remainder] DateTime closeDate) {
            if (closeDate.IsInPast())
                await ReplyAsync(":warning: Given date is in the past!");
            else if (closeDate.IsNoLaterThanDays(1)) await ReplyAsync(":warning: Given date is within less than 24 hours!");

            var mission = SignupsData.GetCurrentlyEditedMission(Context.User.Id);

            if (mission is null) {
                await ReplyAsync(":warning: You are not creating any mission.");
                return;
            }

            if (closeDate < mission.Date) {
                mission.CloseTime = closeDate;
                mission.CustomClose = true;
                await ReplyAsync($"Sign-ups close date set to {closeDate}, in {closeDate.FromNow()}!");
            } else {
                await ReplyAsync(":warning: Sign-ups close date must be before mission date!");
            }
        }

        [Command("add-section", RunMode = RunMode.Async)]
        [Summary("Defining sections in the format: `Callsign | emote [number] optional_slot_name`, where `Callsign` is the name of the section, " +
                 "emote is the emote used to sign up for the role, [number] is the number of slots in a given role. " +
                 "`Zulu example | :cook: [1] or Alpha 1 | :cook: [1] Commander | 🚑 [1] Medic | :onion: [5] PFI` can be given several different emotes. " +
                 "T he order in which the sections are added remains as the order in which they are displayed on the records. " +
                 "Resevring slots is done by adding at the end the role of the person, " +
                 "for example a reserved TL slot in the standard section will look like this  " +
                 "`Alpha 1 | :cook: [1] Commander @Frustrated Zwiebel#2041 | 🚑 [1] Medic | :onion: [4] PFI`.")]
        [ContextDMOrChannel]
        public async Task AddTeam([Remainder]string teamText)
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);

                var slotTexts = teamText.Split("|");

                if (slotTexts.Length > 1)
                {
                    var team = new Mission.Team();
                    team.Name = slotTexts[0];
                    team.Pattern = "";

                    foreach (var slotText in slotTexts)
                    {
                        MatchCollection matches = MiscHelper.GetSlotMatchesFromText(slotText);
                        if (matches.Count == 0)
                            continue;

                        Match match = matches.First();

                        if(match.Success)
                        {
                            var slot = new Mission.Team.Slot(match.Groups[1].Value, int.Parse(match.Groups[2].Value.Substring(1, match.Groups[2].Value.Length - 2)));
                            if(match.Groups.Count == 4)
                            {
                                slot.Name = match.Groups[3].Value;
                                slot.Name = slot.Name.Replace("<@", "<@!");

                                foreach (var user in Context.Message.MentionedUsers)
                                {
                                    if(slot.Name.Contains(user.Mention))
                                    {
                                        slot.Name = slot.Name.Replace(user.Mention, "").TrimEnd();
                                        slot.Signed.Add(user.Id);
                                    }
                                }
                            }
                            team.Slots.Add(slot);

                            if (team.Pattern.Length > 0)
                                team.Pattern += "| ";
                            team.Pattern += $"{slot.Emoji} [{slot.Count}] {slot.Name} ";
                        }
                    }
                    
                   if (team.Slots
                        .GroupBy(x => x.Emoji)
                        .Any(x => x.Count() > 1))
                    {
                        await ReplyAsync("You have doubled a reaction. Correct it.");
                        return;
                    }

                    var embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle(team.Name)
                        .WithDescription(_miscHelper.BuildTeamSlots(team)[0])
                        .WithFooter(team.Pattern);

                    _miscHelper.CreateConfirmationDialog(
                        _dialogs,
                        Context,
                        embed.Build(),
                        dialog =>
                        {
                            Context.Channel.DeleteMessageAsync(dialog.DialogID);
                            _dialogs.Dialogs.Remove(dialog);
                            mission.Teams.Add(team);
                            foreach(var slot in team.Slots)
                            {
                                foreach(var signed in slot.Signed)
                                {
                                    mission.SignedUsers.Add(signed);
                                }
                            }
                            ReplyAsync("OK!");
                        }, 
                        dialog =>
                        {
                            Context.Channel.DeleteMessageAsync(dialog.DialogID);
                            _dialogs.Dialogs.Remove(dialog);
                            ReplyAsync("OK Boomer");
                        });
                }
            }
            else
            {
                await ReplyAsync("To which mission would you like to add this squad?");
            }
        }
        /*
        [Command("add-standard-squad")]
        [Summary("Defines a team with the given name (one word) consisting of SL and two sections, " +
                 "in each section there is a commander, medic and 4 PFI by default. " +
                 "The number of PFI can be changed by giving the total number of people per section as the second parameter.")]
        [ContextDMOrChannel]
        public async Task AddTeam(string teamName, int teamSize = 6)
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);
                // SL
                var team = new Mission.Team();
                team.Name = teamName + " SL | <:cook:> [1] | 🚑 [1]";
                var slot = new Mission.Team.Slot(
                    "Commander",
                    "<:cook:>",
                    1);
                team.Slots.Add(slot);

                slot = new Mission.Team.Slot(
                    "Medic",
                    "🚑",
                    1);
                team.Slots.Add(slot);
                team.Pattern = "<:cook:> [1] | 🚑 [1]";
                mission.Teams.Add(team);

                // team 1
                team = new Mission.Team();
                team.Name = teamName + " 1 | <:cook:> [1] | 🚑 [1] | <:onion:> [" + (teamSize-2) + "]";
                slot = new Mission.Team.Slot(
                    "Commander",
                    "<:cook:>",
                    1);
                team.Slots.Add(slot);

                slot = new Mission.Team.Slot(
                    "Medic",
                    "🚑",
                    1);
                team.Slots.Add(slot);

                slot = new Mission.Team.Slot(
                    "PFI",
                    "<:onion:>",
                    teamSize - 2);
                team.Slots.Add(slot);
                team.Pattern = "<:cook:> [1] | 🚑 [1] | <:onion:> [" + (teamSize - 2) + "]";
                mission.Teams.Add(team);

                // team 2
                team = new Mission.Team();
                team.Name = teamName + " 2 | <:cook:> [1] | 🚑 [1] | <:onion:> [" + (teamSize - 2) + "]";
                slot = new Mission.Team.Slot(
                    "Commander",
                    "<:cook:>",
                    1);
                team.Slots.Add(slot);

                slot = new Mission.Team.Slot(
                    "Medic",
                    "🚑",
                    1);
                team.Slots.Add(slot);

                slot = new Mission.Team.Slot(
                    "PFI",
                    "<:onion:>",
                    teamSize - 2);
                team.Slots.Add(slot);
                team.Pattern = "<:cook:> [1] | 🚑 [1] | <:onion:> [" + (teamSize - 2) + "]";
                mission.Teams.Add(team);

                await ReplyAsync("Something else?");
            }
            else
            {
                await ReplyAsync("Which mission would you like to add this squad to?");
            }
        }
        */
        /*
        [Command("add-reserve")]
        [Summary("Adds a reserve with an unlimited number of spots, " +
                 "when specifying a number in the parameter, it provides such a number of" +
                 "spots on the channel for recruits to sign-up.")]
        [ContextDMOrChannel]
        public async Task AddReserve(int slots = 0)
        {
	        if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
	        {
		        var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);
		        // SL
		        var team = new Mission.Team();
                team.Slots.Add(new Mission.Team.Slot(
	                "Reserve",
                    "🚑",
	                slots));
                team.Pattern = $"Reserve 🚑 [{slots}]";
                mission.Teams.Add(team);

                await ReplyAsync("Something else?");
	        }
	        else
	        {
		        await ReplyAsync("For what is this reserve?");
	        }
        }*/
        
        [Command("edit-section")]
        [Summary("Displays a panel for section ordering and deletion. The arrows move the selection/sections. " +
                 "The pushpin is to \"grab\" a section to move it. Scissors remove the selected section. The padlock completes the section editing.")]
        [ContextDMOrChannel]
        public async Task EditTeams()
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);

                var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("Squads:")
                .WithDescription(MiscHelper.BuildEditTeamsPanel(mission.Teams, mission.HighlightedTeam));

                var message = await Context.Channel.SendMessageAsync(embed: embed.Build());
                mission.EditTeamsMessage = message.Id;
                mission.HighlightedTeam = 0;

                var reactions = new IEmote[5];
                reactions[0] = new Emoji("⬆");
                reactions[1] = new Emoji("⬇");
                reactions[2] = new Emoji("📍");
                reactions[3] = new Emoji("✂");
                reactions[4] = new Emoji("🔒");

                await message.AddReactionsAsync(reactions);
            }
        }

        [Command("ping")]
        [Summary("It allows you to enable/disable pinging everyone for sign-ups.")]
        [ContextDMOrChannel]
        public async Task ToggleMentionEveryone()
        {
            var mission = SignupsData.GetCurrentlyEditedMission(Context.User.Id);

            if (mission is null)
            {
                await ReplyAsync(":warning: You are not creating or editing any missions at the moment.");
                return;
            }

            mission.MentionEveryone = !mission.MentionEveryone;
            if (mission.MentionEveryone)
            {
                await ReplyAsync("Pinging was enabled.");
            }
            else
            {
                await ReplyAsync("Pinging was disabled.");
            }
        }

        [Command("end")]
        [Summary("Displays a confirmation dialogue of the collected mission information.")]
        [ContextDMOrChannel]
        public async Task EndSignups()
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);
                if (SignupHelper.CheckMissionComplete(mission))
                {
                    var embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle(mission.Title)
                        .WithDescription(mission.Description)
                        .WithFooter(mission.Date.ToString())
                        .AddField("Closing time:", mission.CloseTime.ToString())
                        .AddField("Pinging everyone:", mission.MentionEveryone)
                        .WithAuthor(Context.User);

                    if (mission.Attachment != null)
                        embed.WithImageUrl(mission.Attachment);

                    mission.Modlist ??= "Not defined";

                    embed.AddField("Modlist:", mission.Modlist);

                    _miscHelper.BuildTeamsEmbed(mission.Teams, embed);

                    _miscHelper.CreateConfirmationDialog(
                        _dialogs,
                       Context,
                       embed.Build(),
                       dialog =>
                       {
                           _dialogs.Dialogs.Remove(dialog);
                           _ = SignupHelper.CreateSignupChannel(SignupsData, Context.User.Id, Context.Channel);
                           ReplyAsync("Let's go!");
                       },
                       dialog =>
                       {
                           Context.Channel.DeleteMessageAsync(dialog.DialogID);
                           _dialogs.Dialogs.Remove(dialog);
                           ReplyAsync("Correct it quickly!");
                       });
                }
                else
                {
                    await ReplyAsync("You did not give all the information!");
                }
            }
            else
            {
                await ReplyAsync("What do you want to finish, if you haven;t started anything?");
            }
        }

        [Command("loaded")]
        [Summary("Shows missions with active sign-ups, purely debug option.")]
        [ContextDMOrChannel]
        public async Task Loaded()
        {
            foreach(var mission in SignupsData.Missions)
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithTitle(mission.Title)
                    .WithDescription(mission.Description)
                    .WithFooter(mission.Date.ToString())
                    .AddField("Closing time:", mission.CloseTime.ToString())
                    .WithAuthor(_client.GetUser(mission.Owner));

                if (mission.Attachment != null)
                    embed.WithImageUrl(mission.Attachment);

                if (mission.Modlist != null)
                    embed.AddField("Modlist:", mission.Modlist);
                else
                    embed.AddField("Modlist:", "Default");

                _miscHelper.BuildTeamsEmbed(mission.Teams, embed);

                var builtEmbed = embed.Build();

                await ReplyAsync($"{builtEmbed.Length}", embed: builtEmbed);
            }
        }

        [Command("cancel")]
        [Summary("Cancels the creation of the mission, deletes all information defined about it. It does not cancel already made sing-ups.")]
        [ContextDMOrChannel]
        public async Task CancelSignups()
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                SignupsData.Missions.Remove(SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id));

                await ReplyAsync("Noone wants to play your missions anyways.");
            }
            else
                await ReplyAsync("Cancel yourself. You are not editing anything.");
        }

        [Command("current")]
        [Summary("Displays the current sign-ups along with the indexes.")]
        [ContextDMOrChannel]
        public async Task ListMissions()
        {
            if (SignupsData.Missions.Any(x => x.Owner == Context.User.Id && x.Editing == Mission.EditEnum.NotEditing))
            {
                var mainEmbed = new EmbedBuilder()
                            .WithColor(Color.Orange);

                int index = 0;

                foreach (var mission in SignupsData.Missions.Where(x => x.Owner == Context.User.Id && x.Editing == Mission.EditEnum.NotEditing))
                {
                    mainEmbed.AddField(index++.ToString(), mission.Title);
                }

                await ReplyAsync(embed: mainEmbed.Build());
            }
            else
            {
                await ReplyAsync("There are no sing-ups for your missions.");
            }
        }

        [Command("remove-mission")]
        [Summary("After using #mission-channel-name as a parameter, cancels all sign-ups by deleting the sign-up channel.")]
        [ContextDMOrChannel]
        public async Task CancelMission(IGuildChannel channel)
        {
            var missionToBeCancelled = SignupsData.Missions.FirstOrDefault(x => x.SignupChannel == channel.Id);

            if (missionToBeCancelled == null)
            {
                await ReplyAsync("There is no such mission.");
                return;
            }

            if (missionToBeCancelled.Owner != Context.User.Id)
            {
                await ReplyAsync("Don't touch what's not yours, maybe?");
                return;
            }

            await missionToBeCancelled.Access.WaitAsync(-1);
            try
            {
                var chanelToBeDeleted = await channel.Guild.GetTextChannelAsync(channel.Id);
                await chanelToBeDeleted.DeleteAsync();
                await ReplyAsync("Would have had 3 FPS anyways.");
            }
            finally
            {
                missionToBeCancelled.Access.Release();
            }
        }

        [Command("edit-mission")]
        [Summary("After using #mission-channel-name as a parameter, it will enable editing of the given mission (part without squads).")]
        [ContextDMOrChannel]
        public async Task EditMission(IGuildChannel channel)
        {
            var currentlyEditedMission = SignupsData.GetCurrentlyEditedMission(Context.User.Id);

            if (currentlyEditedMission == null)
            {
                var missionToBeEdited = SignupsData.Missions.FirstOrDefault(x => x.SignupChannel == channel.Id);
                if (missionToBeEdited == null)
                {
                    await ReplyAsync("here is no such mission.");
                    return;
                }    
                
                if (missionToBeEdited.Owner != Context.User.Id)
                {
                    await ReplyAsync("Don't touch what's not yours, maybe?");
                    return;
                }

                var serialized = JsonConvert.SerializeObject(missionToBeEdited);
                SignupsData.BeforeEditMissions[Context.User.Id] = JsonConvert.DeserializeObject<ArmaforcesMissionBotSharedClasses.Mission>(serialized);
                missionToBeEdited.Editing = ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started;
                await ReplyAsync($"So `{missionToBeEdited.Title}`. What do you want to change?");
            }
            else
            {
                await ReplyAsync($"Finish editing - `{currentlyEditedMission.Title}` first.");
            }
        }

        [Command("edit-name")]
        [Summary("Edit the name of an already created mission.")]
        [ContextDMOrChannel]
        public async Task MissionName([Remainder] string newTitle)
        {
            if (SignupsData.Missions.Any(x =>
                (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started) &&
                x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x =>
                (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started) &&
                x.Owner == Context.User.Id);

                mission.Title = newTitle;

                await ReplyAsync("So be it...");
            }
            else
            {
                await ReplyAsync("Choose mission to edit first.");
            }
        }

        [Command("edit-save")]
        [Summary("Saves changes to the currently edited mission, if the parameter is set to true, an announcement about changes to the mission will be sent.")]
        [ContextDMOrChannel]
        public async Task SaveChanges(bool announce = false)
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.Started && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.Started && x.Owner == Context.User.Id);

                await mission.Access.WaitAsync(-1);
                try
                {
                    if (SignupHelper.CheckMissionComplete(mission))
                    {
                        var guild = _client.GetGuild(_config.AFGuild);

                        var channel = await SignupHelper.UpdateMission(guild, mission, SignupsData);

                        mission.Editing = Mission.EditEnum.NotEditing;

                        if(announce)
                            await channel.SendMessageAsync("@everyone The mission has been modified, please see the new information and adjust your sign-ups.");

                        await ReplyAsync("Sir, yes Sir!");
                    }
                    else
                    {
                        await ReplyAsync("You haven't given all the information!");
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Oops, something went wrong: {e.Message}");
                }
                finally
                {
                    mission.Access.Release();
                }
            }
        }

        [Command("edit-cancel")]
        [Summary("Cancels the current edition of the mission without saving the changes.")]
        [ContextDMOrChannel]
        public async Task CancelChanges(bool announce = false)
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.Started && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.Started && x.Owner == Context.User.Id);
                
                await mission.Access.WaitAsync(-1);
                try
                {
                    // Don't want to write another function just to copy class, and performance isn't a problem here so just serialize it and deserialize
                    SignupsData.Missions.Remove(mission);
                    var serialized = JsonConvert.SerializeObject(SignupsData.BeforeEditMissions[Context.User.Id]);
                    var oldMission = JsonConvert.DeserializeObject<Mission>(serialized);
                    SignupsData.Missions.Add(oldMission);

                    oldMission.Editing = Mission.EditEnum.NotEditing;
                    await ReplyAsync("Good.");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Oops, something went wrong : {e.Message}");
                }
                finally
                {
                    mission.Access.Release();
                }
            }
        }

        /*[Command("upgrade")]
        [Summary("Performs needed channel upgrades, only Ilddor can use it.")]
        [RequireOwner]
        public async Task Upgrade()
        {
            foreach (var mission in SignupsData.Missions)
            {
                await mission.Access.WaitAsync(-1);
                try
                {
                    Uri uriResult;
                    bool validUrl = Uri.TryCreate(mission.Modlist, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if(!validUrl)
                    {
                        bool recheck = Uri.TryCreate($"https://modlist.armaforces.com/#/download/{mission.Modlist}", UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        if(recheck)
                        {
                            mission.Modlist = $"https://modlist.armaforces.com/#/download/{mission.Modlist}";
                            var guild = _client.GetGuild(_config.AFGuild);
                            var channel = await SignupHelper.UpdateMission(guild, mission, SignupsData);
                            await ReplyAsync($"Mission {mission.Title} has been updated.");
                        }
                    }
                }
                finally
                {
                    mission.Access.Release();
                }
            }

            await ReplyAsync("Done.");

            await BanHelper.MakeBanHistoryMessage(_map, Context.Guild);
        }*/

        /// <summary>
        /// Replies to user with message and throws <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Exception type to throw</typeparam>
        /// <param name="message">Exception message</param>
        /// <returns>Throws <typeparamref name="T"/></returns>
        public async Task ReplyWithException<T>(string message = null) where T : Exception, new()
        {
            // ReSharper disable once PossibleNullReferenceException
            if (message != null)
            {
                await ReplyWithError(message);
                throw (T)Activator.CreateInstance(typeof(T), message);
            }

            var exception = new T();
            await ReplyWithError(exception.Message);
            throw exception;
        }

        private async Task ReplyWithError(string message)
        {
            await ReplyAsync(message ?? "Error when processing command.");
        }
    }
}
