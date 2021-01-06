using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBotSharedClasses;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ArmaforcesMissionBot.Helpers
{
    public class SignupHelper
    {
        private readonly DiscordSocketClient _client;
        private readonly Config _config;
        private readonly MiscHelper _miscHelper;
        private readonly SignupsData _signupsData;

        public SignupHelper(
            DiscordSocketClient client,
            Config config,
            MiscHelper miscHelper,
            SignupsData signupsData)
        {
            _client = client;
            _config = config;
            _miscHelper = miscHelper;
            _signupsData = signupsData;
        }

        public static bool CheckMissionComplete(Mission mission)
        {
            if (mission.Title == null ||
                mission.Description == null ||
                mission.Date == null ||
                mission.Teams.Count == 0  ||
                mission.CloseTime > mission.Date)
                return false;
            else
                return true;
        }

        public async Task<RestTextChannel> CreateChannelForMission(SocketGuild guild, Mission mission, SignupsData signups)
        {
            // Sort channels by date
            signups.Missions.Sort((x, y) =>
            {
                return x.Date.CompareTo(y.Date);
            });

            var signupChannel = await guild.CreateTextChannelAsync(mission.Title, x =>
            {
                x.CategoryId = _config.SignupsCategory;
                // Kurwa dlaczego to nie działa
                var index = (int)(mission.Date - new DateTime(2019, 1, 1)).TotalMinutes;
                // really hacky solution to avoid recalculating indexes for each channel integer should have 
                // space for around 68 years, and this bot is not going to work this long for sure
                x.Position = index;
            });

            var everyone = guild.EveryoneRole;
            var armaforces = guild.GetRole(_config.SignupRole);
            var botRole = guild.GetRole(_config.BotRole);

            var banPermissions = new OverwritePermissions(
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny);

            var botPermissions = new OverwritePermissions(
                PermValue.Deny,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Deny,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Allow,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Allow,
                PermValue.Deny);

            var everyoneStartPermissions = new OverwritePermissions(
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny,
                PermValue.Deny);

            try
            {
                await signupChannel.AddPermissionOverwriteAsync(botRole, botPermissions);

                await signups.BanAccess.WaitAsync(-1);
                try
                {
                    foreach (var ban in signups.SpamBans)
                    {
                        await signupChannel.AddPermissionOverwriteAsync(_client.GetGuild(_config.AFGuild).GetUser(ban.Key), banPermissions);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    signups.BanAccess.Release();
                }

                await signupChannel.AddPermissionOverwriteAsync(everyone, everyoneStartPermissions);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            return signupChannel;
        }

        public static async Task<EmbedBuilder> CreateMainEmbed(SocketGuild guild, Mission mission)
        {
            var mainEmbed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithTitle(mission.Title)
                    .WithDescription(mission.Description)
                    .AddField("Data:", mission.Date.ToString())
                    .AddField("Zamknięcie zapisów:", mission.CloseTime.ToString())
                    .WithAuthor(guild.GetUser(mission.Owner));

            mainEmbed.Author.Url = BotConstants.DISCORD_USER_URL_PREFIX + (mission.Owner).ToString();
            
            if (mission.Attachment != null)
                mainEmbed.WithImageUrl(mission.Attachment);

            if (mission.Modlist != null)
                mainEmbed.AddField("Modlista:", mission.Modlist);
            else
                mainEmbed.AddField("Modlista:", "https://modlist.armaforces.com/#/download/default");

            return mainEmbed;
        }

        public bool ShowMissionToUser(ulong userID, ulong missionID)
        {
            bool showMission = false;

            _signupsData.BanAccess.Wait(-1);
            try
            {
                showMission = !(_signupsData.SignupBans.ContainsKey(userID) && _signupsData.Missions.Any(x => x.SignupChannel == missionID && x.Date < _signupsData.SignupBans[userID]));
            }
            finally
            {
                _signupsData.BanAccess.Release();
            }

            return showMission;
        }

        public async Task CreateMissionMessagesOnChannel(SocketGuild guild, Mission mission, RestTextChannel signupChannel)
        {
            var mainEmbed = await CreateMainEmbed(guild, mission);

            if (mission.AttachmentBytes != null)
            {
                mainEmbed.ImageUrl = $"attachment://{mission.FileName}";
                Stream stream = new MemoryStream(mission.AttachmentBytes);
                var tmpMessage = await signupChannel.SendFileAsync(stream, mission.FileName, "", embed: mainEmbed.Build());
                mission.Attachment = tmpMessage.Embeds.First().Image.Value.Url;
                mission.AttachmentBytes = null;
                mission.FileName = null;
            }
            else
                await signupChannel.SendMessageAsync("", embed: mainEmbed.Build());

            foreach (var team in mission.Teams)
            {
                var description = _miscHelper.BuildTeamSlots(team);

                var teamEmbed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithTitle(team.Name)
                    .WithFooter(team.Pattern);

                if (description.Count == 2)
                    teamEmbed.WithDescription(description[0] + description[1]);
                else if (description.Count == 1)
                    teamEmbed.WithDescription(description[0]);

                var teamMsg = await signupChannel.SendMessageAsync(embed: teamEmbed.Build());
                team.TeamMsg = teamMsg.Id;

                var reactions = new IEmote[team.Slots.Count];
                var num = 0;
                foreach (var slot in team.Slots)
                {
                    try
                    {
                        var emote = Emote.Parse(HttpUtility.HtmlDecode(slot.Emoji));
                        reactions[num++] = emote;
                    }
                    catch (Exception e)
                    {
                        var emoji = new Emoji(HttpUtility.HtmlDecode(slot.Emoji));
                        reactions[num++] = emoji;
                    }
                }
                await teamMsg.AddReactionsAsync(reactions);
            }

            // Make channel visible and notify everyone
            var everyone = guild.EveryoneRole;
            await signupChannel.RemovePermissionOverwriteAsync(everyone);
            if (mission.MentionEveryone)
            {
                await signupChannel.SendMessageAsync("@everyone");
            }
        }

        public async Task<SocketTextChannel> UpdateMission(SocketGuild guild, Mission mission, SignupsData signups)
        {
            // Sort channels by date
            signups.Missions.Sort((x, y) =>
            {
                return x.Date.CompareTo(y.Date);
            });

            var signupChannel = guild.GetChannel(mission.SignupChannel) as SocketTextChannel;

            await signupChannel.ModifyAsync(x =>
            {
                x.CategoryId = _config.SignupsCategory;
                // Kurwa dlaczego to nie działa
                var index = (int)(mission.Date - new DateTime(2019, 1, 1)).TotalMinutes;
                // really hacky solution to avoid recalculating indexes for each channel integer should have 
                // space for around 68 years, and this bot is not going to work this long for sure
                x.Position = index;
                x.Name = mission.Title;
            });

            var mainEmbed = await CreateMainEmbed(guild, mission);

            var messages = signupChannel.GetMessagesAsync(1000);

            await messages.ForEachAsync(x =>
            {
                foreach (var missionMsg in x)
                {
                    if (missionMsg.Embeds.Count != 0 &&
                        missionMsg.Author.Id == _client.CurrentUser.Id)
                    {
                        var embed = missionMsg.Embeds.Single();
                        if (embed.Author != null)
                        {
                            (missionMsg as IUserMessage).ModifyAsync(message => message.Embed = mainEmbed.Build());
                        }
                    }
                }
            });

            return signupChannel;
        }

        public async Task CreateSignupChannel(SignupsData signups, ulong ownerID, ISocketMessageChannel channnel)
        {
            if (signups.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == ownerID))
            {
                var mission = signups.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == ownerID);
                await mission.Access.WaitAsync(-1);
                try
                {
                    if (CheckMissionComplete(mission))
                    {
                        var guild = _client.GetGuild(_config.AFGuild);

                        var signupChannel = await CreateChannelForMission(guild, mission, signups);
                        mission.SignupChannel = signupChannel.Id;

                        await CreateMissionMessagesOnChannel(guild, mission, signupChannel);

                        mission.Editing = Mission.EditEnum.NotEditing;
                    }
                    else
                    {
                        await channnel.SendMessageAsync("Nie uzupełniłeś wszystkich informacji ciołku!");
                    }
                }
                catch (Exception e)
                {
                    await channnel.SendMessageAsync($"Oj, coś poszło nie tak: {e.Message}");
                }
                finally
                {
                    mission.Access.Release();
                }
            }
            else
            {
                await channnel.SendMessageAsync("A może byś mi najpierw powiedział co ty chcesz potwierdzić?");
            }
        }
    }
}
