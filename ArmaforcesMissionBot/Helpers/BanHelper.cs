﻿using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmaforcesMissionBot.Helpers
{
    public class BanHelper
    {
        private readonly SignupsData _signupsData;

        public BanHelper(SignupsData signupsData)
        {
            _signupsData = signupsData;
        }

        public static async Task<ulong> MakeBanMessage(IServiceProvider map, SocketGuild guild, Dictionary<ulong, DateTime> bans, ulong banMessageId, ulong banAnnouncementChannel, string messageText)
        {
            try
            {
                var message = "";

                var list = bans.ToList();

                list.Sort((x, y) => x.Value.CompareTo(y.Value));

                foreach (var ban in list)
                {
                    if (guild.GetUser(ban.Key) != null)
                        message += $"{guild.GetUser(ban.Key).Mention}-{ban.Value.ToString()}\n";
                    else
                        message += $"<@!{ban.Key}>-{ban.Value.ToString()}\n";
                }

                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithDescription(message);

                if (banMessageId != 0)
                {
                    var banAnnouncemens = guild.GetTextChannel(banAnnouncementChannel);
                    var banMessage = await banAnnouncemens.GetMessageAsync(banMessageId) as IUserMessage;
                    await banMessage.ModifyAsync(x => x.Embed = embed.Build());
                    return banMessageId;
                }
                else
                {
                    var banAnnouncemens = guild.GetTextChannel(banAnnouncementChannel);
                    var sentMessage = await banAnnouncemens.SendMessageAsync(messageText, embed: embed.Build());
                    return sentMessage.Id;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"[{DateTime.Now.ToString()}] MakeBanMessageFailed: {e.Message}");
            }

            return banMessageId;
        }

        public static async Task MakeBanHistoryMessage(IServiceProvider map, SocketGuild guild)
        {
            try
            {
                var signups = map.GetService<SignupsData>();
                var config = map.GetService<Config>();

                var message = ""; ;

                foreach (var ban in signups.SignupBansHistory.OrderByDescending(x => x.Value.Item2))
                {
                    if(guild.GetUser(ban.Key) != null)
                        message += $"{guild.GetUser(ban.Key).Mention}-{ban.Value.Item1.ToString()}-{ban.Value.Item2.ToString()}\n";
                    else
                        message += $"<@!{ban.Key}>-{ban.Value.Item1.ToString()}-{ban.Value.Item2.ToString()}\n";
                }

                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithTitle("`nickname-number of bans-number of days banned`")
                    .WithDescription(message);

                if (signups.SignupBansHistoryMessage != 0)
                {
                    var banAnnouncemens = guild.GetTextChannel(config.HallOfShameChannel);
                    var banMessage = await banAnnouncemens.GetMessageAsync(signups.SignupBansHistoryMessage) as IUserMessage;
                    await banMessage.ModifyAsync(x => x.Embed = embed.Build());
                }
                else
                {
                    var banAnnouncemens = guild.GetTextChannel(config.HallOfShameChannel);
                    var sentMessage = await banAnnouncemens.SendMessageAsync("Record of Sign-up bans:", embed: embed.Build());
                    signups.SignupBansHistoryMessage = sentMessage.Id;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"[{DateTime.Now.ToString()}] MakeBanHistoryMessageFailed: {e.Message}");
            }
        }

        public static async Task MakeSpamBanHistoryMessage(IServiceProvider map, SocketGuild guild)
        {
            var signups = map.GetService<SignupsData>();
            var config = map.GetService<Config>();

            var message = "";

            foreach (var ban in signups.SpamBansHistory.OrderByDescending(x=> x.Value.Item1))
            {
                if (guild.GetUser(ban.Key) != null)
                    message += $"{guild.GetUser(ban.Key).Mention}-{ban.Value.Item1.ToString()}-{ban.Value.Item2.ToString()}-{ban.Value.Item3.ToString()}\n";
                else
                    message += $"<@!{ban.Key}>-{ban.Value.Item1.ToString()}-{ban.Value.Item2.ToString()}-{ban.Value.Item3.ToString()}\n";
            }

            var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("`nickname-number of bans-last ban-type of the last ban`")
                .WithDescription(message);

            if (signups.SpamBansHistoryMessage != 0)
            {
                var banAnnouncemens = guild.GetTextChannel(config.HallOfShameChannel);
                var banMessage = await banAnnouncemens.GetMessageAsync(signups.SpamBansHistoryMessage) as IUserMessage;
                await banMessage.ModifyAsync(x => x.Embed = embed.Build());
            }
            else
            {
                var banAnnouncemens = guild.GetTextChannel(config.HallOfShameChannel);
                var sentMessage = await banAnnouncemens.SendMessageAsync("Record of Spam bans:", embed: embed.Build());
                signups.SpamBansHistoryMessage = sentMessage.Id;
            }
        }

        public static async Task UnsignUser(IServiceProvider map, SocketGuild guild, SocketUser user)
        {
            var signups = map.GetService<SignupsData>();
            var config = map.GetService<Config>();

            foreach (var mission in signups.Missions)
            {
                await mission.Access.WaitAsync(-1);
                try
                {
                    if (mission.Date < signups.SignupBans[user.Id] && mission.SignedUsers.Contains(user.Id))
                    {
                        foreach (var team in mission.Teams)
                        {
                            if(team.Slots.Any(x => x.Signed.Contains(user.Id)))
                            {
                                var channel = guild.GetTextChannel(mission.SignupChannel);
                                var message = await channel.GetMessageAsync(team.TeamMsg) as IUserMessage;
                                IEmote reaction;
                                try
                                {
                                    reaction = Emote.Parse(team.Slots.Single(x => x.Signed.Contains(user.Id)).Emoji);
                                }
                                catch (Exception e)
                                {
                                    reaction = new Emoji(team.Slots.Single(x => x.Signed.Contains(user.Id)).Emoji);
                                }
                                await message.RemoveReactionAsync(reaction, user);
                            }
                        }
                    }
                }
                finally
                {
                    mission.Access.Release();
                }
            }
        }

        public static async Task BanUserSpam(IServiceProvider map, IUser user)
        {
            var signups = map.GetService<SignupsData>();
            var config = map.GetService<Config>();
            var client = map.GetService<DiscordSocketClient>();

            if (signups.SpamBansHistory.ContainsKey(user.Id) && signups.SpamBansHistory[user.Id].Item2.AddDays(1) > DateTime.Now)
            {
                var banEnd = DateTime.Now;
                switch (signups.SpamBansHistory[user.Id].Item3)
                {
                    case SignupsData.BanType.Godzina:
                        signups.SpamBans.Add(user.Id, DateTime.Now.AddDays(1));
                        signups.SpamBansHistory[user.Id] = new Tuple<uint, DateTime, SignupsData.BanType>(
                            signups.SpamBansHistory[user.Id].Item1 + 1,
                            DateTime.Now.AddDays(1),
                            SignupsData.BanType.Dzień);
                        break;
                    case SignupsData.BanType.Dzień:
                    case SignupsData.BanType.Tydzień:
                        signups.SpamBans.Add(user.Id, DateTime.Now.AddDays(7));
                        signups.SpamBansHistory[user.Id] = new Tuple<uint, DateTime, SignupsData.BanType>(
                            signups.SpamBansHistory[user.Id].Item1 + 1,
                            DateTime.Now.AddDays(7),
                            SignupsData.BanType.Tydzień);
                        break;
                }
            }
            else
            {
                signups.SpamBans.Add(user.Id, DateTime.Now.AddHours(1));
                if (signups.SpamBansHistory.ContainsKey(user.Id))
                {
                    signups.SpamBansHistory[user.Id] = new Tuple<uint, DateTime, SignupsData.BanType>(
                                signups.SpamBansHistory[user.Id].Item1 + 1,
                                DateTime.Now.AddHours(1),
                                SignupsData.BanType.Godzina);
                }
                else
                {
                    signups.SpamBansHistory[user.Id] = new Tuple<uint, DateTime, SignupsData.BanType>(
                                1,
                                DateTime.Now.AddHours(1),
                                SignupsData.BanType.Godzina);
                }
            }

            var guild = client.GetGuild(config.AFGuild);
            var contemptChannel = guild.GetTextChannel(config.PublicContemptChannel);
            switch (signups.SpamBansHistory[user.Id].Item3)
            {
                case SignupsData.BanType.Godzina:
                    await user.SendMessageAsync("Your access has been removed for an hour because of spam protection.");
                    await contemptChannel.SendMessageAsync($"User {user.Mention} has been temporarily (one hour) banned from sign-ups (spam protection).");
                    break;
                case SignupsData.BanType.Dzień:
                    await user.SendMessageAsync("Your access has been removed for one day because of spam protection.");
                    await contemptChannel.SendMessageAsync($"User {user.Mention} has been temporarily (one day) banned from sign-ups (spam protection)");
                    break;
                case SignupsData.BanType.Tydzień:
                    await user.SendMessageAsync("Your access has been removed for one week because of spam protection.");
                    await contemptChannel.SendMessageAsync($"User {user.Mention} has been temporarily (one week) banned from sign-ups (spam protection)");
                    break;
            }

            signups.SpamBansMessage = await Helpers.BanHelper.MakeBanMessage(
                map,
                guild,
                signups.SpamBans,
                signups.SpamBansMessage,
                config.HallOfShameChannel,
                "Spam bans:");

            await Helpers.BanHelper.MakeSpamBanHistoryMessage(map, guild);

            foreach (var mission in signups.Missions)
            {
                var missionChannel = guild.GetTextChannel(mission.SignupChannel);
                try
                {
                    await missionChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(
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
                    PermValue.Deny));
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Woops, banning user from channel failed : {e.Message}");
                }
            }
        }

        public bool IsUserSpamBanned(ulong userID)
        {
            bool isBanned = true;

            _signupsData.BanAccess.Wait(-1);
            try
            {
                isBanned = _signupsData.SpamBans.ContainsKey(userID);
            }
            finally
            {
                _signupsData.BanAccess.Release();
            }

            return isBanned;
        }
    }
}
