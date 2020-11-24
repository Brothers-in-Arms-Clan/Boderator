using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Features.ServerManager.Server.DTOs;
using CSharpFunctionalExtensions;
using RestSharp;

namespace ArmaforcesMissionBot.Features.ServerManager.Server
{
    public class ServerManagerClient : ManagerClientBase, IServerManagerClient
    {
        // Port is const as Manager doesn't support multiple servers _yet_.
        private const int Port = 2302;

        private string ServerApiPath { get; } = "api/server";
        
        public ServerManagerClient(Config config) : base(config.ServerManagerUrl)
        {
        }

        public Result<ServerStatus> GetServerStatus()
        {
            var resource = string.Join(
                '/',
                ServerApiPath,
                Port);
            var restRequest = new RestRequest(resource, Method.GET);

            var result = ManagerClient.Execute<ServerStatus>(restRequest);
            return result.IsSuccessful
                ? Result.Success(result.Data)
                : ReturnFailureFromResponse<ServerStatus>(result);
        }

        public Result RequestStartServer(ServerStartRequest serverStartRequest)
        {
            var resource = string.Join(
                '/',
                ServerApiPath,
                "start");
            var restRequest = new RestRequest(resource, Method.POST);
            restRequest.AddJsonBody(serverStartRequest);

            var result = ManagerClient.Execute(restRequest);
            return result.IsSuccessful
                ? Result.Success()
                : ReturnFailureFromResponse<ServerStatus>(result);
        }
    }

    public interface IServerManagerClient
    {
        Result<ServerStatus> GetServerStatus();

        Result RequestStartServer(ServerStartRequest serverStartRequest);
    }
}
