using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Features.Signups.Missions;
using Discord;
using Discord.WebSocket;

namespace ArmaforcesMissionBot.Features.RichPresence {
    /// <summary>
    /// Handles updating activity bar with signups data.
    /// </summary>
    public class GameStatusUpdater {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly SignupsData _signupsData;

        private Timer _timer;
        private int _statusCounter;
        private Game _currentStatus;

        public GameStatusUpdater(
            DiscordSocketClient discordSocketClient,
            SignupsData signupsData
        ) {
            _signupsData = signupsData;
            _discordSocketClient = discordSocketClient;
        }

        public void StartTimer() {
            _timer?.Dispose();
            _timer = new Timer {Interval = 5000};

            _timer.Elapsed += UpdateStatus;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void UpdateStatus(object sender, ElapsedEventArgs e)
        {
            var missionsWithSignups = _signupsData.Missions
                .Where(x => x.Editing == Mission.EditEnum.NotEditing)
                .ToList();
            var missionsWithFreeSlots = missionsWithSignups
                .Where(mission => Helpers.MiscHelper.CountFreeSlots(mission) != 0)
                .ToList();
            var signupsCount = missionsWithSignups.Count;
            var missionsWithFreeSlotsCount = missionsWithFreeSlots.Count;

            Game status;
            if (signupsCount == 0)
                status = CreateNoSignupsDetails();
            else if (missionsWithFreeSlotsCount == 0)
                status = CreateSignupsNoFreeSlotsDetails(missionsWithFreeSlotsCount, signupsCount);
            else if (_statusCounter < missionsWithFreeSlotsCount)
                status = CreateMissionSignupDetails(missionsWithFreeSlots.ElementAt(_statusCounter));
            else
                status = CreateSignupsCounterDetails(missionsWithFreeSlotsCount, signupsCount);

            _statusCounter = AdjustCounter(missionsWithFreeSlotsCount);

            if (status.Name == _currentStatus?.Name) return;

            _currentStatus = status;
            _discordSocketClient.SetActivityAsync(_currentStatus);
        }

        private int AdjustCounter(int missionsWithFreeSlotsCount) 
            => _statusCounter >= missionsWithFreeSlotsCount
                ? 0
                : ++_statusCounter;

        private static Game CreateSignupsNoFreeSlotsDetails(int freeSlots, int allSignups) 
            => new Game($"Brak miejsc w zapisach. {freeSlots}/{allSignups}");

        private static Game CreateNoSignupsDetails() 
            => new Game($"Brak prowadzonych zapisów.");

        private static Game CreateMissionSignupDetails(Mission mission) 
            => new Game($"Miejsc: {Helpers.MiscHelper.CountFreeSlots(mission)}/{Helpers.MiscHelper.CountAllSlots(mission)} - {mission.Title}");

        private static Game CreateSignupsCounterDetails(int freeSlots, int allSignups) 
            => new Game($"Zapisy: {freeSlots}/{allSignups}");
    }
}
