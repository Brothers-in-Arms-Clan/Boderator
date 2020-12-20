using System;
using System.Collections.Generic;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Extensions;
using ArmaforcesMissionBot.Features.Modsets.Constants;
using CSharpFunctionalExtensions;
using RestSharp;

namespace ArmaforcesMissionBot.Features.Modsets
{
    internal class ModsetsApiClient : IModsetsApiClient
    {
        private readonly IRestClient _restClient;

        public string ApiUrl { get; }
        
        public ModsetsApiClient(Config config) : this(config.ModsetsApiUrl)
        {
        }

        public ModsetsApiClient(string baseUrl) : this(new Uri(baseUrl))
        {
            ApiUrl = baseUrl;
        }

        private ModsetsApiClient(Uri baseUri)
        {
            _restClient = new RestClient(baseUri);
        }

        /// <inheritdoc />
        public Result<List<WebModset>> GetModsets()
            => ApiModsets();

        /// <inheritdoc />
        public Result<WebModset> GetModsetDataByName(string name)
            => ApiModsetByName(name);

        private Result<WebModset> ApiModsetByName(string name)
        {
            var requestUri = ModsetsApiConstants.ApiByNamePath(name);
            var request = new RestRequest(requestUri, Method.GET, DataFormat.Json);
            return _restClient.ExecuteAndReturnData<WebModset>(request);
        }

        private Result<List<WebModset>> ApiModsets()
        {
            var requestUri = ModsetsApiConstants.ApiPath;
            var request = new RestRequest(requestUri, Method.GET, DataFormat.Json);
            return _restClient.ExecuteAndReturnData<List<WebModset>>(request);
        }
    }
}
