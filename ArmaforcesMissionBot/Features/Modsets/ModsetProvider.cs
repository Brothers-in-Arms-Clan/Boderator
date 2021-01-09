using System.Linq;
using ArmaforcesMissionBot.Features.Modsets.Constants;
using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Features.Modsets
{
    public class ModsetProvider : IModsetProvider
    {
        private readonly IModsetsApiClient _modsetsApiClient;

        public ModsetProvider(IModsetsApiClient modsetsApiClient)
        {
            _modsetsApiClient = modsetsApiClient;
        }

        public Result ModsetWithNameExists(string modsetName)
        {
            return _modsetsApiClient.GetModsetDataByName(modsetName).Match(
                onSuccess: _ => Result.Success(),
                onFailure: Result.Failure);
        }

        public string GetModsetNameFromUrl(string modsetNameOrUrl)
        {
            return modsetNameOrUrl.Contains('/')
                ? modsetNameOrUrl.Split('/').Last()
                : modsetNameOrUrl;
        }

        public Result<string> GetModsetDownloadUrl(string modsetName)
        {
            return _modsetsApiClient.GetModsetDataByName(modsetName).Match(
                onSuccess: modset => Result.Success(ModsetsApiConstants.DownloadPageForModset(_modsetsApiClient.ApiUrl, modset.Name)),
                onFailure: Result.Failure<string>);
        }
    }
}
