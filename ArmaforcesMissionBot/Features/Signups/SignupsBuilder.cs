using System;
using ArmaforcesMissionBot.Features.Signups.Missions;
using ArmaforcesMissionBot.Features.Signups.Missions.Validators;
using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups
{
    public class SignupsBuilder : ISignupsBuilder
    {
        private readonly IMissionValidator _missionValidator;

        private Mission _mission;

        public SignupsBuilder(IMissionValidator missionValidator)
        {
            _missionValidator = missionValidator;
        }

        public ISignupsBuilder CreateNewMission()
        {
            _mission = new Mission
            {
                Editing = Mission.EditEnum.New
            };

            return this;
        }

        public ISignupsBuilder LoadMission(IMission mission)
        {
            _mission = mission as Mission;

            if (_mission is null)
            {
                throw new Exception("Mission could not be loaded for edition.");
            }

            _mission.Editing = Mission.EditEnum.Started;

            return this;
        }

        public ISignupsBuilder SetMissionTitle(string title)
        {
            _mission.Title = title;
            return this;
        }

        public ISignupsBuilder SetMissionOwner(ulong userId)
        {
            _mission.Owner = userId;
            return this;
        }

        public ISignupsBuilder SetMissionDescription(string description, Attachment attachment)
        {
            _mission.Description = description;
            _mission.Attachment = attachment.Url;
            return this;
        }

        public ISignupsBuilder SetModset(string modsetName, string modsetUrl)
        {
            _mission.Modlist = _mission.ModlistUrl = modsetUrl;
            _mission.ModlistName = modsetName;
            return this;
        }

        public ISignupsBuilder SetDate(DateTime dateTime)
        {
            _mission.Date = dateTime;
            
            if (!_mission.CustomClose)
            {
                _mission.CloseTime = dateTime.AddMinutes(-60);
            }

            return this;
        }

        public ISignupsBuilder SetCloseDate(DateTime dateTime)
        {
            // TODO: Validate date >= close date.
            _mission.CloseTime = dateTime;
            _mission.CustomClose = true;
            return this;
        }

        public ISignupsBuilder MentionEveryone(bool shouldMentionEveryone = true)
        {
            _mission.MentionEveryone = shouldMentionEveryone;
            return this;
        }

        public Result ValidateMission()
        {
            return _missionValidator.CheckSignupsComplete(_mission);
        }

        public IMission Build()
        {
            return _mission;
        }
    }

    public interface ISignupsBuilder
    {
        ISignupsBuilder CreateNewMission();

        ISignupsBuilder LoadMission(IMission mission);

        ISignupsBuilder SetMissionOwner(ulong userId);

        ISignupsBuilder SetMissionTitle(string title);

        ISignupsBuilder SetMissionDescription(string description, Attachment attachment);

        ISignupsBuilder SetModset(string modsetName, string modsetUrl);

        ISignupsBuilder SetDate(DateTime dateTime);

        ISignupsBuilder SetCloseDate(DateTime dateTime);

        ISignupsBuilder MentionEveryone(bool shouldMentionEveryone);

        Result ValidateMission();

        IMission Build();
    }
}
