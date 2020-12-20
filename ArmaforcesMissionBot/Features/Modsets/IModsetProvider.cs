using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Features.Modsets
{
    public interface IModsetProvider
    {
        Result<string> GetModsetDownloadUrl(string modsetName);
    }
}
