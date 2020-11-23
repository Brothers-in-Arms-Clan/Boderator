using System;
using ArmaforcesMissionBot.Features.ServerManager.Server.DTOs;
using CSharpFunctionalExtensions;

namespace ArmaforcesMissionBot.Features.ServerManager.Server.Extensions
{
    public static class ServerManagerClientExtensions
    {
        public static Result RequestStartServer(
            this IServerManagerClient serverManagerClient,
            string modsetName,
            DateTime? dateTime)
        {
            var request = new ServerStartRequest
            {
                ModsetName = modsetName,
                ScheduleAt = dateTime
            };

            return serverManagerClient.RequestStartServer(request);
        }
    }
}
