using ArmaforcesMissionBot.DataClasses;

namespace ArmaforcesMissionBot.Features.Modsets.Constants
{
    internal class ModsetsApiConstants
    {
        public const string ApiPath = "api/mod-lists";

        public static string ApiByNamePath(string modsetName) => $"{ApiPath}/by-name/{modsetName}";

        public static string DownloadPageForModset(string apiUrl, string modsetName) => $"{apiUrl}/mod-list/{modsetName}";
    }
}
