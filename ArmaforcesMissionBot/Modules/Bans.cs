using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Helpers;

namespace ArmaforcesMissionBot.Modules
{
    [Name("Bans")]
    public class Bans : ModuleBase<SocketCommandContext>
    {
        public IServiceProvider _map { get; set; }
        public DiscordSocketClient _client { get; set; }
        public Config _config { get; set; }
        public CommandService _commands { get; set; }

        public MiscHelper MiscHelper { get; set; }

        public Bans()
        {
        }

        [Command("ban")]
        [Summary("Ban the given person from signing up until the given date. The number of days of the ban can be given as the second argument, the default is 7.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task BanSignups(SocketUser user, uint days = 7)
        {
            var signups = _map.GetService<SignupsData>();

            await signups.BanAccess.WaitAsync(-1);

            try
            {
                var banEnd = DateTime.Now.AddDays(days);
                banEnd = banEnd.AddHours(23 - banEnd.Hour);
                banEnd = banEnd.AddMinutes(59 - banEnd.Minute);
                banEnd = banEnd.AddSeconds(59 - banEnd.Second);
                signups.SignupBans.Add(user.Id, banEnd);
                if (signups.SignupBansHistory.ContainsKey(user.Id))
                {
                    signups.SignupBansHistory[user.Id] = new Tuple<uint, uint>(
                        signups.SignupBansHistory[user.Id].Item1 + 1,
                        signups.SignupBansHistory[user.Id].Item2 + days);
                }
                else
                    signups.SignupBansHistory[user.Id] = new Tuple<uint, uint>(1, days);

                signups.SignupBansMessage = await Helpers.BanHelper.MakeBanMessage(
                    _map, 
                    Context.Guild, 
                    signups.SignupBans, 
                    signups.SignupBansMessage, 
                    _config.HallOfShameChannel, 
                    "Sign-up bans:");

                await Helpers.BanHelper.MakeBanHistoryMessage(_map, Context.Guild);
                
                var embedBuilder = new EmbedBuilder()
                {
                    ImageUrl = _config.BanImageUrl
                };
                await ReplyAsync("Die, Die, Die My Darling.",
                embed: embedBuilder.Build());

                await Helpers.BanHelper.UnsignUser(_map, Context.Guild, user);
            }
            finally
            {
                signups.BanAccess.Release();
            }
        }

        [Command("unban")]
        [Summary("Unbans the person.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task UnbanSignups(SocketUser user)
        {
            var signups = _map.GetService<SignupsData>();

            await signups.BanAccess.WaitAsync(-1);

            try
            {
                if (signups.SignupBans.ContainsKey(user.Id))
                {
                    signups.SignupBans.Remove(user.Id);
                    signups.SignupBansMessage = await Helpers.BanHelper.MakeBanMessage(
                        _map, 
                        Context.Guild, 
                        signups.SignupBans, 
                        signups.SignupBansMessage, 
                        _config.HallOfShameChannel, 
                        "Sign-up bans:");
                    await ReplyAsync("You're just too nice...");
                }
            }
            finally
            {
                signups.BanAccess.Release();
            }
        }

        [Command("banSpam")]
        [Summary("Spam ban")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task BanSpam(SocketUser user)
        {
            var signups = _map.GetService<SignupsData>();

            await signups.BanAccess.WaitAsync(-1);

            try
            {
                await Helpers.BanHelper.BanUserSpam(_map, user);
                await ReplyAsync("You little shit you...");
            }
            finally
            {
                signups.BanAccess.Release();
            }
        }

        [Command("unbanSpam")]
        [Summary("Removes Spam ban.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task UnbanSpam(SocketUser user)
        {
            var signups = _map.GetService<SignupsData>();

            await signups.BanAccess.WaitAsync(-1);

            try
            {
                if (signups.SpamBans.ContainsKey(user.Id))
                {
                    signups.SpamBans.Remove(user.Id);
                    signups.SpamBansMessage = await Helpers.BanHelper.MakeBanMessage(
                        _map,
                        Context.Guild,
                        signups.SpamBans,
                        signups.SpamBansMessage,
                        _config.HallOfShameChannel,
                        "Spam bans:");

                    // Remove permissions override from channels
                    if (signups.Missions.Count > 0)
                    {
                        foreach (var mission in signups.Missions)
                        {
                            var channel = Context.Guild.GetTextChannel(mission.SignupChannel);
                            await channel.RemovePermissionOverwriteAsync(user);
                        }
                    }
                    await ReplyAsync("Be happy you have FPS not FPM...");
                }
            }
            finally
            {
                signups.BanAccess.Release();
            }
        }

        [Command("unsign")]
        [Summary("Unsigns the user from the mission")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task Unsign(ulong userID, IMessageChannel channel)
        {
            var signups = _map.GetService<SignupsData>();
            //var user = _client.GetUser(userID);

            if (signups.Missions.Any(x => x.SignupChannel == channel.Id))
            {
                var mission = signups.Missions.Single(x => x.SignupChannel == channel.Id);

                Console.WriteLine($"[{DateTime.Now.ToString()}] {userID} removed from mission {channel.Name} by {Context.User.Username}");

                await mission.Access.WaitAsync(-1);
                try
                {
                    foreach(var team in mission.Teams)
                    {
                        var teamMsg = await channel.GetMessageAsync(team.TeamMsg) as IUserMessage;
                        var embed = teamMsg.Embeds.Single();

                        if(team.Slots.Any(x => x.Signed.Contains(userID)))
                        {
                            team.Slots.Single(x => x.Signed.Contains(userID)).Signed.Remove(userID);
                            mission.SignedUsers.Remove(userID);

                            var newDescription = MiscHelper.BuildTeamSlots(team);

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
                        }
                    }
                }
                finally
                {
                    mission.Access.Release();
                }
            }
        }
    }
}
