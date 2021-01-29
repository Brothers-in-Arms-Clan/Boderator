using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Features.Modsets
{
    public interface IModsetsApiClient
    {
        public string ApiUrl { get; }

        Result<List<WebModset>> GetModsets();

        Result<WebModset> GetModsetDataByName(string name);
    }
}