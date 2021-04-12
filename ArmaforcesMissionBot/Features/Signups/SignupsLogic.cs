using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Configuration.Constants;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Extensions;
using ArmaforcesMissionBot.Features.Modsets;
using ArmaforcesMissionBot.Features.Signups.Missions;
using ArmaforcesMissionBot.Helpers;
using ArmaforcesMissionBot.Providers.Guild;
using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups
{
    public class SignupsLogic : ISignupsLogic
    {
        private readonly SignupsData _signupsData;
        private readonly SignupHelper _signupHelper;
        private readonly IGuildProvider _guildProvider;
        private readonly IModsetProvider _modsetProvider;
        private readonly ISignupsBuilderDictionary _signupsBuilderDictionary;

        public SignupsLogic(SignupsData signupsData, SignupHelper signupHelper, IGuildProvider guildProvider, IModsetProvider modsetProvider, ISignupsBuilderDictionary signupsBuilderDictionary)
        {
            _signupsData = signupsData;
            _signupHelper = signupHelper;
            _guildProvider = guildProvider;
            _modsetProvider = modsetProvider;
            _signupsBuilderDictionary = signupsBuilderDictionary;
        }

        public Result<string> StartSignupsCreation(IUser user, string title)
        {
            return _signupsBuilderDictionary.StartSignupsCreation(user, title);
        }

        public Result<string> SetModset(IUser user, string modsetNameOrUrl)
        {
            // TODO: Handle both in-progress and just creating signups
            var signupsBuilderResult = _signupsBuilderDictionary.GetSignupsBuilder(user);
            if (signupsBuilderResult.IsFailure) return signupsBuilderResult.ConvertFailure<string>();

            var signupsBuilder = signupsBuilderResult.Value;
            var modsetName = _modsetProvider.GetModsetNameFromUrl(modsetNameOrUrl);
            return _modsetProvider.GetModsetDownloadUrl(modsetName)
                .Match(
                    onSuccess: url =>
                    {
                        url = url.Replace(" ", "%20");
                        signupsBuilder.SetModset(modsetName, url);
                        return Result.Success($"Modset {modsetName} was found under {url}.");
                    },
                    onFailure: Result.Failure<string>);
        }

        public Result<List<string>> SetCloseDate(
            IUser user,
            DateTime closeDate,
            bool forceInvalidDate = false)
        {
            var messages = new List<string>();

            return CheckDateIsValid(closeDate, forceInvalidDate)
                .Tap(message => messages.Add(message))
                .Bind(_ => _signupsBuilderDictionary.GetSignupsBuilder(user))
                .Tap(signupsBuilder => signupsBuilder.SetCloseDate(closeDate))
                .Tap(_ => messages.Add("Fill remaining mission info."))
                .Bind(_ => Result.Success(messages));
        }

        public Result<string> SetDescription(IUser user, string description, Attachment? attachment)
        {
            return _signupsBuilderDictionary.GetSignupsBuilder(user)
                .Tap(signupsBuilder => signupsBuilder.SetMissionDescription(description, attachment))
                .Bind(_ => Result.Success("Fill remaining mission info."));
        }

        public Result<List<string>> SetDate(IUser user, DateTime date, bool forceInvalidDate = false)
        {
            var messages = new List<string>();

            return CheckDateIsValid(date, forceInvalidDate)
                .Tap(message => messages.Add(message))
                .Bind(_ => _signupsBuilderDictionary.GetSignupsBuilder(user))
                .Tap(signupsBuilder => signupsBuilder.SetDate(date))
                .Tap(_ => messages.Add("Fill remaining mission info."))
                .Bind(_ => Result.Success(messages));
        }

        public Result<string> SetMissionName(IUser user, string newName)
        {
            return _signupsBuilderDictionary.GetSignupsBuilder(user)
                .Tap(signupsBuilder => signupsBuilder.SetMissionTitle(newName))
                .Bind(signupsBuilder => Result.Success($"Mission renamed to {signupsBuilder.Build().Title}."));
        }

        public Result<string> ToggleMentionEveryone(IUser user)
        {
            return _signupsBuilderDictionary.GetSignupsBuilder(user)
                .Bind(ToggleMentionEveryone);
        }

        public Result<string> StartSignupsEdition(IUser user, IGuildChannel channel)
        {
            return _signupsBuilderDictionary.CheckNoMissionIsEdited(user)
                .Bind(() => _signupsData.GetUserMissionForEdition(user, channel))
                .Tap(mission => _signupsBuilderDictionary.EditSignups(user, mission))
                .Bind(mission => Result.Success($"Started edition of {mission.Title}, what to change?"));
        }

        public Result<string> CancelSignupsEdition(IUser user)
        {
            return _signupsBuilderDictionary.RemoveSignupsBuilder(user)
                .Bind(signupsBuilder => Result.Success($"Mission {signupsBuilder.Build().Title} edition aborted."));
        }

        public async Task<Result<string>> FinishSignupsEdition(IUser user)
        {
            var mission = _signupsData.GetCurrentlyEditedMission(user.Id);

            return await _signupsBuilderDictionary.GetSignupsBuilder(user)
                .Tap(_ => mission.Access.WaitAsync(-1))
                .Bind(signupsBuilder => signupsBuilder.ValidateMission())
                .Bind(() => Result.Success(_signupHelper.UpdateMission(_guildProvider.GetSocketGuild(), mission, _signupsData)))
                .Bind(_ => _signupsBuilderDictionary.RemoveSignupsBuilder(user))
                .Finally(ReleaseMissionAccessSemaphore<ISignupsBuilder>(mission))
                .Bind(_ => Result.Success($"Mission {mission.Title} edition finished successfully."));
        }

        private static Result<string> ToggleMentionEveryone(ISignupsBuilder signupsBuilder)
        {
            var mission = signupsBuilder.Build() as Mission;
            if (mission!.Editing == Mission.EditEnum.Started)
            {
                return Result.Failure<string>("You can only toggle mention for new signups.");
            }

            var newMentionEveryone = !mission.MentionEveryone;
            signupsBuilder.MentionEveryone(newMentionEveryone);

            var enabled = newMentionEveryone
                ? "Enabled"
                : "Disabled";

            return Result.Success($"{enabled} mentioning everyone for {mission.Title}.");
        }

        private static Result<string> CheckDateIsValid(DateTime date, bool forceInvalidDate = false)
        {
            if (forceInvalidDate)
            {
                return Result.Success($"Forced date to {date}, in {date.FromNow()}.");
            }

            if (date.IsInPast())
            {
                return Result.Failure<string>(":warning: Provided date is in the past! Signups would close immediately." +
                                              " If you wish to force that, add 'true' after date.");
            }

            return date.IsNoLaterThanDays(SignupsConstants.MinimumSignupsDurationInDays)
                ? Result.Failure<string>($":warning: Provided date is no later than {SignupsConstants.MinimumSignupsDurationInDays} days.")
                : Result.Success($"Set date to {date}, in {date.FromNow()}.");
        }

        private static Func<Result<T>, Result<T>> ReleaseMissionAccessSemaphore<T>(Mission mission) => result =>
        {
            mission.Access.Release();
            return result;
        };
    }
}
