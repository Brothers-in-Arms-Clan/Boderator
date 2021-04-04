using System.Collections.Concurrent;
using System.Collections.Generic;
using ArmaforcesMissionBot.Features.Signups.Missions;
using CSharpFunctionalExtensions;
using Discord;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Features.Signups
{
    public class SignupsBuilderDictionary : ISignupsBuilderDictionary
    {
        private readonly ISignupsBuilderFactory _signupsBuilderFactory;
        private readonly ConcurrentDictionary<ulong, ISignupsBuilder> _signupsBuilders = new ConcurrentDictionary<ulong, ISignupsBuilder>();

        public SignupsBuilderDictionary(ISignupsBuilderFactory signupsBuilderFactory)
        {
            _signupsBuilderFactory = signupsBuilderFactory;
        }

        public Result<string> StartSignupsCreation(IUser user, string title)
        {
            return CheckNoMissionIsEdited(user)
                .Tap(() => CreateNewSignups(user, title))
                // TODO: Get missing stuff and print it.
                .Bind(() => Result.Success("Fill remaining mission info."));
        }

        public ISignupsBuilder CreateNewSignups(IUser user, string title) =>
            _signupsBuilders[user.Id] = _signupsBuilderFactory.CreateSignupsBuilder()
                .SetMissionTitle(title)
                .SetMissionOwner(user.Id);

        public void EditSignups(IUser user, Mission missionToBeEdited)
        {
            // Create mission copy
            var serialized = JsonConvert.SerializeObject(missionToBeEdited);
            var mission = JsonConvert.DeserializeObject<Mission>(serialized);

            _signupsBuilders[user.Id] = _signupsBuilderFactory.CreateSignupsBuilder()
                .LoadMission(mission);
        }

        public Result<ISignupsBuilder> RemoveSignupsBuilder(IUser user)
        {
            var success = _signupsBuilders.Remove(user.Id, out var signupsBuilder);

            return success
                ? Result.Success(signupsBuilder)
                : Result.Failure<ISignupsBuilder>($"Failed to stop signups edition for user {user.Mention}. No mission is being edited.");
        }

        public Result<ISignupsBuilder> GetSignupsBuilder(IUser user)
        {
            var signupsBuilder = GetExistingSignupsBuilderForUser(user);

            return signupsBuilder is null
                ? Result.Failure<ISignupsBuilder>("No mission edition or creation is in progress.")
                : Result.Success(signupsBuilder);
        }

        public Result CheckNoMissionIsEdited(IUser user)
        {
            var signupsBuilder = GetExistingSignupsBuilderForUser(user);

            return signupsBuilder != null
                ? Result.Failure<string>($"Edition or creation of {signupsBuilder.Build().Title} is already in progress.")
                : Result.Success();
        }

        public ISignupsBuilder GetExistingSignupsBuilderForUser(IUser user)
            => _signupsBuilders.ContainsKey(user.Id)
                ? _signupsBuilders[user.Id]
                : null;
    }
}
