using ArmaforcesMissionBot.Features.Signups.Missions;
using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups
{
    public interface ISignupsBuilderDictionary
    {
        Result<string> StartSignupsCreation(IUser user, string title);

        ISignupsBuilder CreateNewSignups(IUser user, string title);

        void EditSignups(IUser user, Mission mission);

        Result<ISignupsBuilder> RemoveSignupsBuilder(IUser user);

        Result<ISignupsBuilder> GetSignupsBuilder(IUser user);

        Result CheckNoMissionIsEdited(IUser user);

        ISignupsBuilder GetExistingSignupsBuilderForUser(IUser user);
    }
}
