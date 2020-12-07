using ArmaforcesMissionBot.DataClasses;
using RestSharp;
using RestSharp.Authenticators;

namespace ArmaforcesMissionBot.Features.ServerManager
{
    public class ApiKeyAuthenticator : IAuthenticator
    {
        private readonly Config _config;
        private const string HeaderField = "ApiKey";

        public ApiKeyAuthenticator(Config config)
        {
            _config = config;
        }

        public void Authenticate(IRestClient client, IRestRequest request) => 
            request.AddHeader(HeaderField, _config.ServerManagerApiKey);
    }
}
