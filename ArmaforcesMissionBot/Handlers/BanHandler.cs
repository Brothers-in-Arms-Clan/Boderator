using ArmaforcesMissionBot.DataClasses;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace ArmaforcesMissionBot.Handlers
{
    public class BanHandler : IInstallable
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private Config _config;
        private Timer _timer;
        private SignupsData _signupsData;

        public async Task Install(IServiceProvider map)
        {
            _client = map.GetService<DiscordSocketClient>();
            _config = map.GetService<Config>();
            _signupsData = map.GetService<SignupsData>();
            _services = map;
            // Hook the MessageReceived event into our command handler
            _timer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = 60000
            };

            _timer.Elapsed += CheckBans;
        }

        private async void CheckBans(object source, ElapsedEventArgs e)
        {
            await _signupsData.BanAccess.WaitAsync(-1);

            try
            {
                if (_signupsData.SignupBans.Count > 0)
                {
                    List<ulong> toRemove = new List<ulong>();
                    foreach (var ban in _signupsData.SignupBans)
                    {
                        if (ban.Value < e.SignalTime)
                        {
                            toRemove.Add(ban.Key);
                        }
                    }
                    foreach(var removeID in toRemove)
                    {
                        _signupsData.SignupBans.Remove(removeID);
                    }
                    _signupsData.SignupBansMessage = await Helpers.BanHelper.MakeBanMessage(
                                _services,
                                _client.GetGuild(_config.AFGuild),
                                _signupsData.SignupBans,
                                _signupsData.SignupBansMessage,
                                _config.HallOfShameChannel,
                                "Bany na zapisy:");
                }
                if(_signupsData.SpamBans.Count > 0)
                {
                    List<ulong> toRemove = new List<ulong>();
                    var guild = _client.GetGuild(_config.AFGuild);
                    foreach (var ban in _signupsData.SpamBans)
                    {
                        if (ban.Value < e.SignalTime)
                        {
                            toRemove.Add(ban.Key);
                            var user = _client.GetUser(ban.Key);
                            if (_signupsData.Missions.Count > 0)
                            {
                                foreach (var mission in _signupsData.Missions)
                                {
                                    var channel = guild.GetTextChannel(mission.SignupChannel);
                                    await channel.RemovePermissionOverwriteAsync(user);
                                }
                            }
                        }
                    }
                    foreach (var removeID in toRemove)
                    {
                        _signupsData.SpamBans.Remove(removeID);
                    }
                    _signupsData.SpamBansMessage = await Helpers.BanHelper.MakeBanMessage(
                        _services, 
                        guild,
                        _signupsData.SpamBans,
                        _signupsData.SpamBansMessage,
                        _config.HallOfShameChannel,
                        "Bany za spam reakcjami:");
                }
            }
            finally
            {
                _signupsData.BanAccess.Release();
            }
        }
    }
}
