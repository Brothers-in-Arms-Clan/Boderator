using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Features.Modsets
{
    public interface IModsetProvider
    {
        string GetModsetNameFromUrl(string modsetNameOrUrl);

        Result<string> GetModsetDownloadUrl(string modsetName);
    }
}
