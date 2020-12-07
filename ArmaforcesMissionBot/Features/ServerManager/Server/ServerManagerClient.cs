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
        
        public ServerManagerClient(Config config) : base(config)
        {
        }

        public Result<ServerStatus> GetServerStatus()
        {
            var resource = string.Join(
                '/',
                ServerApiPath,
                Port);
            var restRequest = new RestRequest(resource, Method.GET);

            var response = ManagerClient.Execute<ServerStatus>(restRequest);
            return response.IsSuccessful
                ? Result.Success(response.Data)
                : ReturnFailureFromResponse<ServerStatus>(response);
        }

        public Result RequestStartServer(ServerStartRequest serverStartRequest)
        {
            var resource = string.Join(
                '/',
                ServerApiPath,
                "start");
            var restRequest = new RestRequest(resource, Method.POST);
            restRequest.AddJsonBody(serverStartRequest);

            var response = ManagerClient.Execute(restRequest);
            return response.IsSuccessful
                ? Result.Success()
                : ReturnFailureFromResponse<ServerStatus>(response);
        }
    }

    public interface IServerManagerClient
    {
        Result<ServerStatus> GetServerStatus();

        Result RequestStartServer(ServerStartRequest serverStartRequest);
    }
}
