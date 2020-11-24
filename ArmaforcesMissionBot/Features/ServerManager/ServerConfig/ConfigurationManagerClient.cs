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

        public Result<string> GetServerConfiguration()
        {
            var restClient = new RestClient(ManagerUrl);

            var resource = string.Join(
                '/',
                ConfigurationApiPath,
                "server");
            var restRequest = new RestRequest(resource, Method.GET);

            var result = restClient.Execute(restRequest);
            return result.IsSuccessful
                ? Result.Success(result.Content)
                : ReturnFailureFromResponse<string>(result);
        }

        public Result<string> GetModsetConfiguration(string modsetName)
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
                ? Result.Success(result.Content)
                : ReturnFailureFromResponse<string>(result);
        }

        public Result<string> PutServerConfiguration(string configContent)
        {
            var restClient = new RestClient(ManagerUrl);
            
            var resource = string.Join(
                '/',
                ConfigurationApiPath,
                "server");

            var restRequest = new RestRequest(resource, Method.PUT);

            restRequest.AddFileBytes(
                "file",
                Encoding.UTF8.GetBytes(configContent),
                "config.json",
                "application/json");

            var result = restClient.Execute<string>(restRequest);

            return result.IsSuccessful
                ? Result.Success(result.Content)
                : ReturnFailureFromResponse<string>(result);
        }

        public Result<string> PutModsetConfiguration(string modsetName, string configContent)
        {
            var restClient = new RestClient(ManagerUrl);
            
            var resource = string.Join(
                '/',
                ConfigurationApiPath,
                "modset",
                modsetName);

            var restRequest = new RestRequest(resource, Method.PUT);

            restRequest.AddFileBytes(
                "file",
                Encoding.UTF8.GetBytes(configContent),
                "config.json",
                "application/json");

            var result = restClient.Execute<string>(restRequest);

            return result.IsSuccessful
                ? Result.Success(result.Content)
                : ReturnFailureFromResponse<string>(result);
        }
    }

    public interface IConfigurationManagerClient
    {
        Result<string> GetServerConfiguration();

        Result<string> GetModsetConfiguration(string modsetName);

        Result<string> PutServerConfiguration(string configContent);

        Result<string> PutModsetConfiguration(string modsetName, string configContent);
    }
}
