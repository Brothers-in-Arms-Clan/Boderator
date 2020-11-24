using System;
using System.Threading.Tasks;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Features.ServerManager.Mods.DTOs;
using CSharpFunctionalExtensions;
using RestSharp;

namespace ArmaforcesMissionBot.Features.ServerManager.Mods
{
    public class ModsManagerClient : ManagerClientBase, IModsManagerClient
    {
        private string ModsApiPath { get; } = "api/mods";

        public ModsManagerClient(Config config) : base(config.ServerManagerUrl)
        {
        }

        public async Task<Result> UpdateMods(string modsetName, DateTime? scheduleAt)
        {
            var resource = string.Join(
                '/',
                ModsApiPath,
                "update");
            var restRequest = new RestRequest(resource, Method.POST);
            var modsUpdateRequest = new ModsUpdateRequest
            {
                ModsetName = modsetName,
                ScheduleAt = scheduleAt
            };
            restRequest.AddJsonBody(modsUpdateRequest);

            var response = await ManagerClient.ExecuteAsync(restRequest);
            return response.IsSuccessful
                ? Result.Success(response)
                : ReturnFailureFromResponse(response);
        }
    }

    public interface IModsManagerClient
    {
        Task<Result> UpdateMods(string modsetName, DateTime? scheduleAt);
    }
}
