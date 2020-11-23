using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Features.ServerManager.Server;
using ArmaforcesMissionBot.Features.ServerManager.Server.DTOs;
using ArmaforcesMissionBot.Features.ServerManager.ServerConfig;
using ArmaforcesMissionBot.Features.ServerManager.Server.Extensions;
using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Modules
{
    [Name("Server")]
    public class Server : ModuleBase<SocketCommandContext>
    {
        public IServerManagerClient ServerManagerClient { get; set; }
        public IConfigurationManagerClient ConfigurationManagerClient { get; set; }

        //public Server(IServerManagerClient serverManagerClient, IConfigurationManagerClient configurationManagerClient)
        //{
            //ServerManagerClient = serverManagerClient;
            //ConfigurationManagerClient = configurationManagerClient;
        //}

        [Command("startServer")]
        [Summary("Pozwala uruchomić serwer z zadanym modsetem o zadanej godzinie w danym dniu. Na przykład: AF!startServer default 2020-07-17T19:00.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task StartServer(string modsetName, DateTime? dateTime)
        {
            var result = ServerManagerClient.RequestStartServer(modsetName, dateTime);

            await result.Match(
                onSuccess: () => ReplyAsync($"Server startup scheduled {(dateTime.HasValue ? $"at {dateTime.Value}" : "now")}."),
                onFailure: error => ReplyAsync(error));
        }

        [Command("serverStatus")]
        [Summary("Sprawdza status serwera.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task ServerStatus()
        {
            var result = ServerManagerClient.GetServerStatus();
            
            await result.Match(
                onSuccess: serverStatus => ReplyAsync(embed: CreateServerStatusEmbed(serverStatus)),
                onFailure: error => ReplyAsync(error));
        }
        

        [Command("serverConfig")]
        [Summary("Pobiera główny config serwera.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task ServerConfig()
        {
            var result = ConfigurationManagerClient.GetServerConfiguration();
            
            result.Match(
                onSuccess: stream => Context.Channel.SendFileAsync(stream, "serverConfig.json"),
                onFailure: error => ReplyAsync(error));
        }

        private static Embed CreateServerStatusEmbed(ServerStatus serverStatus)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = "Server Status",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = nameof(serverStatus.ModsetName),
                        Value = serverStatus.ModsetName ?? "No modset"
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = nameof(serverStatus.IsServerRunning),
                        Value = serverStatus.IsServerRunning
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = nameof(serverStatus.Port),
                        Value = serverStatus.Port
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = nameof(serverStatus.HeadlessClientsConnected),
                        Value = serverStatus.HeadlessClientsConnected ?? 0
                    }
                }
            };

            return embedBuilder.Build();
        }
    }
}
