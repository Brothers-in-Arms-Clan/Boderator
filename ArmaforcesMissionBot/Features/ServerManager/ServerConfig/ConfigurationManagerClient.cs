using System.Text;
using ArmaforcesMissionBot.DataClasses;
using CSharpFunctionalExtensions;
using RestSharp;

namespace ArmaforcesMissionBot.Features.ServerManager.ServerConfig
{
    public class ConfigurationManagerClient : ManagerClientBase, IConfigurationManagerClient
    {
        private string ConfigurationApiPath { get; } = "api/configuration";

        public ConfigurationManagerClient(Config config) : base(config)
        {
        }

        public Result<string> GetServerConfiguration()
        {
            var resource = string.Join(
                '/',
                ConfigurationApiPath,
                "server");
            var restRequest = new RestRequest(resource, Method.GET);

            var response = ManagerClient.Execute(restRequest);
            return response.IsSuccessful
                ? Result.Success(response.Content)
                : ReturnFailureFromResponse<string>(response);
        }

        public Result<string> GetModsetConfiguration(string modsetName)
        {
            var resource = string.Join(
                '/',
                ConfigurationApiPath,
                "modset",
                modsetName);
            var restRequest = new RestRequest(resource, Method.GET);

            var response = ManagerClient.Execute(restRequest);
            return response.IsSuccessful
                ? Result.Success(response.Content)
                : ReturnFailureFromResponse<string>(response);
        }

        public Result<string> PutServerConfiguration(string configContent)
        {
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

            var response = ManagerClient.Execute<string>(restRequest);

            return response.IsSuccessful
                ? Result.Success(response.Content)
                : ReturnFailureFromResponse<string>(response);
        }

        public Result<string> PutModsetConfiguration(string modsetName, string configContent)
        {
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

            var response = ManagerClient.Execute<string>(restRequest);

            return response.IsSuccessful
                ? Result.Success(response.Content)
                : ReturnFailureFromResponse<string>(response);
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
