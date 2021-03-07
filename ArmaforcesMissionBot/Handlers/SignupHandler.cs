using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ArmaforcesMissionBot.Helpers;

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
            var reactionStringAnimatedVersion = reaction.Emote.ToString().Insert(1, "a");

            if (reaction.User.IsSpecified && !reaction.User.Value.IsBot && _signupsData.Missions.Any(x => x.SignupChannel == channel.Id))
            {
                var mission = _signupsData.Missions.Single(x => x.SignupChannel == channel.Id);

                await HandleReactionChange(message, channel, reaction);
                Console.WriteLine($"[{DateTime.Now.ToString()}] {reaction.User.Value.Username} added reaction {reaction.Emote.Name}");

                if (_signupsData.SignupBans.ContainsKey(reaction.User.Value.Id) && _signupsData.SignupBans[reaction.User.Value.Id] > mission.Date)
                {
                    await reaction.User.Value.SendMessageAsync("Masz bana na zapisy, nie możesz zapisać się na misję, która odbędzie się w czasie trwania bana.");
                    var teamMsg = await channel.GetMessageAsync(message.Id) as IUserMessage;
                    await teamMsg.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }

                await mission.Access.WaitAsync(-1);
                try
                {
                    if (mission.Teams.Any(x => x.TeamMsg == message.Id))
                    {
                        var team = mission.Teams.Single(x => x.TeamMsg == message.Id);
                        if (team.Slots.Any(x => (x.Emoji == reaction.Emote || x.Emoji.ToString() == reactionStringAnimatedVersion) && (x.Count > x.Signed.Count() || team.Reserve != 0)))
                        {
                            var teamMsg = await channel.GetMessageAsync(message.Id) as IUserMessage;

                            var embed = teamMsg.Embeds.Single();

                            if (mission.SignedUsers.All(x => x != reaction.User.Value.Id))
                            {
                                var slot = team.Slots.Single(x => x.Emoji == reaction.Emote || x.Emoji.ToString() == reactionStringAnimatedVersion);
                                if(!slot.Signed.Contains(reaction.User.Value.Id))
                                    slot.Signed.Add(reaction.User.Value.Id);
                                if (!mission.SignedUsers.Contains(reaction.User.Value.Id))
                                    mission.SignedUsers.Add(reaction.User.Value.Id);

                                var newDescription = _miscHelper.BuildTeamSlots(team);

                                var newEmbed = new EmbedBuilder
                                {
                                    Title = team.Name,
                                    Color = embed.Color
                                };

                                if (newDescription.Count == 2)
                                    newEmbed.WithDescription(newDescription[0] + newDescription[1]);
                                else if (newDescription.Count == 1)
                                    newEmbed.WithDescription(newDescription[0]);

                                if (embed.Footer.HasValue)
                                    newEmbed.WithFooter(embed.Footer.Value.Text);
                                else
                                    newEmbed.WithFooter(team.Pattern);

                                await teamMsg.ModifyAsync(x => x.Embed = newEmbed.Build());
                            }
                            else
                            {
                                await teamMsg.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                            }
                        }
                        else if (team.Slots.Any(x => x.Emoji == reaction.Emote))
                        {
                            var teamMsg = await channel.GetMessageAsync(message.Id) as IUserMessage;
                            await teamMsg.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                        }
                    }
                }
                finally
                {
                    mission.Access.Release();
                }
            }
            else if(_signupsData.Missions.Any(x => x.SignupChannel == channel.Id) && reaction.UserId != _client.CurrentUser.Id)
            {
                var user = _client.GetUser(reaction.UserId);
                Console.WriteLine($"Naprawiam reakcje po spamie {user.Username}");
                var teamMsg = await channel.GetMessageAsync(message.Id) as IUserMessage;
                await teamMsg.RemoveReactionAsync(reaction.Emote, user);
            }
        }

        private async Task HandleReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var reactionStringAnimatedVersion = reaction.Emote.ToString().Insert(1, "a");

            if (_signupsData.Missions.Any(x => x.SignupChannel == channel.Id))
            {
                var mission = _signupsData.Missions.Single(x => x.SignupChannel == channel.Id);
                var user = await (channel as IGuildChannel).Guild.GetUserAsync(reaction.UserId);

                Console.WriteLine($"[{DateTime.Now.ToString()}] {user.Username} removed reaction {reaction.Emote.Name}");

                await mission.Access.WaitAsync(-1);
                try
                {
                    if (mission.Teams.Any(x => x.TeamMsg == message.Id))
                    {
                        var team = mission.Teams.Single(x => x.TeamMsg == message.Id);
                        if (team.Slots.Any(x => (x.Emoji == reaction.Emote || x.Emoji.ToString() == reactionStringAnimatedVersion) && x.Signed.Contains(user.Id)))
                        {
                            var teamMsg = await channel.GetMessageAsync(message.Id) as IUserMessage;
                            var embed = teamMsg.Embeds.Single();

                            var slot = team.Slots.Single(x => x.Emoji == reaction.Emote || x.Emoji.ToString() == reactionStringAnimatedVersion);
                            slot.Signed.Remove(user.Id);
                            mission.SignedUsers.Remove(user.Id);

                            var newDescription = _miscHelper.BuildTeamSlots(team);

                            var newEmbed = new EmbedBuilder
                            {
                                Title = team.Name,
                                Color = embed.Color
                            };

                            if (newDescription.Count == 2)
                                newEmbed.WithDescription(newDescription[0] + newDescription[1]);
                            else if (newDescription.Count == 1)
                                newEmbed.WithDescription(newDescription[0]);

                            if (embed.Footer.HasValue)
                                newEmbed.WithFooter(embed.Footer.Value.Text);
                            else
                                newEmbed.WithFooter(team.Pattern);

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

                Console.WriteLine($"[{ DateTime.Now.ToString()}] { reaction.User.Value.Username} spam counter: { _signupsData.ReactionTimes[reaction.User.Value.Id].Count}");

                if (_signupsData.ReactionTimes[reaction.User.Value.Id].Count >= 10 && !_signupsData.SpamBans.ContainsKey(reaction.User.Value.Id))
                {
                    await Helpers.BanHelper.BanUserSpam(_services, reaction.User.Value);
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
