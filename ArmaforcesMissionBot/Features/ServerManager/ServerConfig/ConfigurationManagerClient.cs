using System.IO;
using System.Text;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Features.ServerManager.Server.DTOs;
using CSharpFunctionalExtensions;
using RestSharp;

namespace ArmaforcesMissionBot.Features.ServerManager.ServerConfig
{
    public class ConfigurationManagerClient : ManagerClientBase, IConfigurationManagerClient
    {
        private string ConfigurationApiPath { get; } = "api/configuration";

        public ConfigurationManagerClient(Config config) : base(config.ServerManagerUrl)
        {
        }

        public Result<Stream> GetServerConfiguration()
        {
            var restClient = new RestClient(ManagerUrl);

            var resource = string.Join(
                '/',
                ConfigurationApiPath,
                "server");
            var restRequest = new RestRequest(resource, Method.GET);

            var result = restClient.Execute(restRequest);
            return result.IsSuccessful
                ? Result.Success(CreateStreamFromJsonString(result.Content))
                : ReturnFailureFromResponse<Stream>(result);
        }

        public Result<Stream> GetModsetConfiguration(string modsetName)
        {
            var restClient = new RestClient(ManagerUrl);

            var resource = string.Join(
                '/',
                ConfigurationApiPath,
                "modset",
                modsetName);
            var restRequest = new RestRequest(resource, Method.GET);

            var result = restClient.Execute(restRequest);
            return result.IsSuccessful
                ? Result.Success(CreateStreamFromJsonString(result.Content))
                : ReturnFailureFromResponse<Stream>(result);
        }

        private static Stream CreateStreamFromJsonString(string jsonString)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            
            return new MemoryStream(bytes);
        }
    }

    public interface IConfigurationManagerClient
    {
        Result<Stream> GetServerConfiguration();

        Result<Stream> GetModsetConfiguration(string modsetName);
    }
}
