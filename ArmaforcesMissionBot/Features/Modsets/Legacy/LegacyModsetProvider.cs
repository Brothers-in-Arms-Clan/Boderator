using System;
using System.Linq;
using System.Net;
using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Features.Modsets.Legacy
{
    public class LegacyModsetProvider : IModsetProvider
    {
        public Result ModsetWithNameExists(string modsetName)
        {
            var request = WebRequest.Create($"https://server.armaforces.com:8888/modsets/{modsetName}.csv");
            try
            {
                _ = request.GetResponse();
            }
            catch (Exception)
            {
                return Result.Failure("Modlist doesn't exist or the link in incorrect.");
            }

            return Result.Success();
        }

        public string GetModsetNameFromUrl(string modsetNameOrUrl)
        {
            return modsetNameOrUrl.Contains('/')
                ? modsetNameOrUrl.Split('/').Last()
                : modsetNameOrUrl;
        }

        public Result<string> GetModsetDownloadUrl(string modsetName) 
            => $"https://modlist.armaforces.com/#/download/{modsetName}";
    }
}
