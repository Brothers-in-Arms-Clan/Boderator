using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ArmaforcesMissionBot.Features.Bans.Extensions;
using ArmaforcesMissionBot.Features.Signups.Missions;
using ArmaforcesMissionBot.Features.Signups.Missions.Extensions;
using ArmaforcesMissionBot.Features.Signups.Missions.Slots.Extensions;
using ArmaforcesMissionBot.Features.Users.Extensions;
using ArmaforcesMissionBot.Helpers;
using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Handlers
{
    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
    public class SignupHandler : IInstallable
    {
        private DiscordSocketClient _client;
        private MiscHelper _miscHelper;
        private SignupsData _signupsData;
        private IServiceProvider _services;
        private Config _config;
        private Timer _timer;

        public async Task Install(IServiceProvider map)
        {
            _client = map.GetService<DiscordSocketClient>();
            _config = map.GetService<Config>();
            _miscHelper = map.GetService<MiscHelper>();
            _signupsData = map.GetService<SignupsData>();
            _services = map;
            // Hook the MessageReceived event into our command handler
            _client.ReactionAdded += HandleReactionAdded;
            _client.ReactionRemoved += HandleReactionRemoved;
            _client.ChannelDestroyed += HandleChannelRemoved;

            _timer = new Timer
            {
                Interval = 2000
            };

            _timer.Elapsed += CheckReactionTimes;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var user = reaction.User.IsSpecified
                ? reaction.User.Value
                : _client.GetUser(reaction.UserId);
            user ??= await (channel as IGuildChannel).Guild.GetUserAsync(reaction.UserId);

            var emote = reaction.Emote;

            if (user.IsCurrentUser(_client))
            {
                return;
            }

            if (!(await channel.GetMessageAsync(message.Id) is IUserMessage teamMsg))
            {
                throw new Exception("Message for reaction could not be found.");
            }

            var embed = teamMsg.Embeds.SingleOrDefault();
            if (embed is null)
            {
                throw new Exception("Message is missing embed.");
            }

            if (user.IsBot)
            {
                await teamMsg.RemoveReactionAsync(reaction.Emote, user);
                return;
            }

            await HandleReactionChange(message, channel, reaction);
            Console.WriteLine($"[{DateTime.Now}] {user.Username} added reaction {emote.Name}");

            var mission = _signupsData.Missions.Single(x => x.SignupChannel == channel.Id);

            if (_signupsData.UserHasBan(user, mission.Date))
            {
                await user.SendMessageAsync("Masz bana na zapisy, nie możesz zapisać się na misję, która odbędzie się w czasie trwania bana.");
                await teamMsg.RemoveReactionAsync(emote, user);
                return;
            }

            await mission.Access.WaitAsync(-1);
            try
            {
                var team = mission.Teams.SingleOrDefault(x => x.TeamMsg == message.Id);
                if (team is null)
                {
                    await teamMsg.RemoveReactionAsync(emote, user);
                    return;
                }

                var slot = team.GetSlot(emote);
                if (slot is null)
                {
                    await teamMsg.RemoveReactionAsync(emote, user);
                    return;
                }

                await slot.SignUser(user)
                    .Bind(() => mission.SignUser(user))
                    .Bind(() => CreateUpdatedTeamEmbed(team, embed))
                    .Tap(async newEmbed => await teamMsg.ModifyAsync(x => x.Embed = newEmbed))
                    .OnFailure(async error => await teamMsg.RemoveReactionAsync(emote, user));
            }
            finally
            {
                mission.Access.Release();
            }
        }

        private async Task HandleReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var user = reaction.User.IsSpecified
                ? reaction.User.Value
                : _client.GetUser(reaction.UserId);
            user ??= await (channel as IGuildChannel).Guild.GetUserAsync(reaction.UserId);

            var emote = reaction.Emote;

            if (user.IsCurrentUser(_client))
            {
                return;
            }

            if (!(await channel.GetMessageAsync(message.Id) is IUserMessage teamMsg))
            {
                throw new Exception("Message for reaction could not be found.");
            }

            var mission = _signupsData.Missions.Single(x => x.SignupChannel == channel.Id);

            Console.WriteLine($"[{DateTime.Now}] {user.Username} removed reaction {reaction.Emote.Name}");

            await mission.Access.WaitAsync(-1);
            try
            {
                var team = mission.Teams.SingleOrDefault(x => x.TeamMsg == message.Id);
                if (team is null)
                {
                    await teamMsg.RemoveReactionAsync(emote, user);
                    return;
                }

                var embed = teamMsg.Embeds.Single();

                var slot = team.GetSlot(emote);

                await slot.UnsignUser(user)
                    .Bind(() => mission.UnsignUser(user))
                    .Bind(() => CreateUpdatedTeamEmbed(team, embed))
                    .Tap(async newEmbed => await teamMsg.ModifyAsync(x => x.Embed = newEmbed))
                    .OnFailure(async error => await teamMsg.RemoveReactionAsync(emote, user));
            }
            finally
            {
                mission.Access.Release();
            }
        }

        private Result<Embed> CreateUpdatedTeamEmbed(Team team, IEmbed embed)
        {
            var newDescription = _miscHelper.BuildTeamSlots(team);

            var embedBuilder = new EmbedBuilder
            {
                Title = team.Name,
                Color = embed.Color
            };

            if (newDescription.Count == 2)
                embedBuilder.WithDescription(newDescription[0] + newDescription[1]);
            else if (newDescription.Count == 1)
                embedBuilder.WithDescription(newDescription[0]);

            embedBuilder.WithFooter(
                embed.Footer.HasValue
                    ? embed.Footer.Value.Text
                    : team.Pattern);

            return Result.Success(embedBuilder.Build());
        }

        private async Task HandleChannelRemoved(SocketChannel channel)
        {
            if (_signupsData.Missions.Any(x => x.SignupChannel == channel.Id))
            {
                _signupsData.Missions.RemoveAll(x => x.SignupChannel == channel.Id);
            }
        }

        private async Task HandleReactionChange(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await _signupsData.BanAccess.WaitAsync(-1);
            try
            {
                if (!_signupsData.ReactionTimes.ContainsKey(reaction.User.Value.Id))
                {
                    _signupsData.ReactionTimes[reaction.User.Value.Id] = new Queue<DateTime>();
                }

                _signupsData.ReactionTimes[reaction.User.Value.Id].Enqueue(DateTime.Now);

                Console.WriteLine($"[{ DateTime.Now}] { reaction.User.Value.Username} spam counter: { _signupsData.ReactionTimes[reaction.User.Value.Id].Count}");

                if (_signupsData.ReactionTimes[reaction.User.Value.Id].Count >= 10 && !_signupsData.SpamBans.ContainsKey(reaction.User.Value.Id))
                {
                    await BanHelper.BanUserSpam(_services, reaction.User.Value);
                }
            }
            finally
            {
                _signupsData.BanAccess.Release();
            }
        }

        private async void CheckReactionTimes(object source, ElapsedEventArgs e)
        {
            await _signupsData.BanAccess.WaitAsync(-1);
            try
            {
                foreach(var user in _signupsData.ReactionTimes)
                {
                    while (user.Value.Count > 0 && user.Value.Peek() < e.SignalTime.AddSeconds(-30))
                        user.Value.Dequeue();
                }
            }
            finally
            {
                _signupsData.BanAccess.Release();
            }
        }
    }
}
