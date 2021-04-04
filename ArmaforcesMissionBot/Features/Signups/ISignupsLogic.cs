using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups
{
    public interface ISignupsLogic
    {
        Result<string> StartSignupsCreation(IUser user, string title);

        Result<List<string>> SetCloseDate(IUser user, DateTime closeDate, bool forceInvalidDate = false);

        Result<List<string>> SetDate(IUser user, DateTime date, bool forceInvalidDate);

        Result<string> SetDescription(IUser user, string description, Attachment attachment);

        Result<string> SetMissionName(IUser user, string newName);

        Result<string> SetModset(IUser user, string modsetNameOrUrl);

        Result<string> ToggleMentionEveryone(IUser user);

        Result<string> StartSignupsEdition(IUser user, IGuildChannel channel);

        Result<string> CancelSignupsEdition(IUser user);

        Task<Result<string>> FinishSignupsEdition(IUser user);
    }
}
