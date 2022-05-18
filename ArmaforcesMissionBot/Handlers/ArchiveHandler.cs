﻿using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ArmaforcesMissionBot.Helpers;

namespace ArmaforcesMissionBot.Handlers
{
    public class ArchiveHandler : IInstallable
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private MiscHelper _miscHelper;
        private Config _config;
        private Timer _timer;

        public async Task Install(IServiceProvider map)
        {
            _client = map.GetService<DiscordSocketClient>();
            _config = map.GetService<Config>();
            _miscHelper = map.GetService<MiscHelper>();
            _services = map;
            // Hook the MessageReceived event into our command handler
            _timer = new Timer();
            _timer.Interval = 60000;

            _timer.Elapsed += CheckMissionsToArchiive;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void CheckMissionsToArchiive(object source, ElapsedEventArgs e)
        {
            var signups = _services.GetService<SignupsData>();

            if (signups.Missions.Count == 0)
                return;

            foreach (var mission in signups.Missions)
            {
                await mission.Access.WaitAsync(-1);
                try
                {
                    if(mission.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.NotEditing && mission.CloseTime < e.SignalTime)
                    {
                        var archive = _client.GetChannel(_config.SignupsArchive) as ITextChannel;
                        var archiveEmbed = new EmbedBuilder()
                            .WithColor(Color.Green)
                            .WithTitle(mission.Title)
                            .WithDescription(mission.Description)
                            .WithFooter(mission.Date.ToString())
                            .AddField("Date:", mission.Date)
                            .AddField("Closing time:", mission.CloseTime.ToString())
                            .WithAuthor(_client.GetUser(mission.Owner).Username, url: BotConstants.DISCORD_USER_URL_PREFIX + (mission.Owner).ToString())
                            .AddField("Modlist:", mission.Modlist);

                        if (mission.Attachment != null)
                            archiveEmbed.WithImageUrl(mission.Attachment);

                        var channel = _client.GetChannel(mission.SignupChannel) as ITextChannel;

                        _miscHelper.BuildTeamsEmbed(mission.Teams, archiveEmbed, true);

                        await archive.SendMessageAsync(embed: archiveEmbed.Build());

                        await channel.DeleteAsync();
                        signups.Missions.Remove(mission);

                        var archiveMission = new MissionsArchiveData.Mission();
                        archiveMission.Title = mission.Title;
                        archiveMission.Date = mission.Date;
                        archiveMission.CloseTime = mission.CloseTime;
                        archiveMission.Description = mission.Description;
                        archiveMission.Modlist = mission.Modlist;
                        archiveMission.Attachment = mission.Attachment;
                        archiveMission.FreeSlots = (ulong)Helpers.MiscHelper.CountFreeSlots(mission);
                        archiveMission.AllSlots = (ulong)Helpers.MiscHelper.CountAllSlots(mission);

                        var archiveMissions = _services.GetService<MissionsArchiveData>();

                        archiveMissions.ArchiveMissions.Add(archiveMission);

                        break;
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
