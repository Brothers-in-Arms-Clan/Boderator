using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        
        [Command("modsetConfig")]
        [Summary("Pobiera config serwera dla danego modsetu.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task ModsetConfig(string modsetName)
        {
            var result = ConfigurationManagerClient.GetModsetConfiguration(modsetName);
            
            await result.Match(
                onSuccess: config => ReplyWithConfigContent(config, modsetName),
                onFailure: error => ReplyAsync(error));
        }
        
        [Command("putModsetConfig")]
        [Summary("Wrzuca config serwera dla danego modsetu.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task PutModsetConfig(string modsetName, [Remainder]string configContent = null)
        {
            if (Context.Message.Attachments.Any(x => x.Filename.EndsWith(".json")))
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(
                    Context.Message.Attachments
                        .First(x => x.Filename.EndsWith(".json"))
                        .Url);
                configContent = await response.Content.ReadAsStringAsync();
            }

            if (configContent is null)
            {
                await ReplyAsync("No configuration specified or attached file has incorrect extension. Only *.json files are allowed.");
                return;
            }

            var result = ConfigurationManagerClient.PutModsetConfiguration(modsetName, configContent);
            
            await result.Match(
                onSuccess: modset => ReplyAsync($"Configuration for {modset} modset updated."),
                onFailure: error => ReplyAsync(error));
        }
        
        [Command("serverConfig")]
        [Summary("Pobiera główny config serwera.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task ServerConfig()
        {
            var result = ConfigurationManagerClient.GetServerConfiguration();
            
            await result.Match(
                onSuccess: config => ReplyWithConfigContent(config),
                onFailure: error => ReplyAsync(error));
        }

        [Command("putServerConfig")]
        [Summary("Wrzuca główny config serwera.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task PutServerConfig([Remainder] string configContent = null)
        {
            if (Context.Message.Attachments.Any(x => x.Filename.EndsWith(".json")))
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(
                    Context.Message.Attachments
                        .First(x => x.Filename.EndsWith(".json"))
                        .Url);
                configContent = await response.Content.ReadAsStringAsync();
            }

            if (configContent is null)
            {
                await ReplyAsync("No configuration specified or attached file has incorrect extension. Only *.json files are allowed.");
                return;
            }

            var result = ConfigurationManagerClient.PutServerConfiguration(configContent);

            await result.Match(
                onSuccess: modset => ReplyAsync($"Configuration for {modset} modset updated."),
                onFailure: error => ReplyAsync(error));
        }

        private async Task ReplyWithConfigContent(string configString, string modsetName = "server")
        {
            if (configString.Length > 1900)
            {
                await Context.Channel.SendFileAsync(
                    CreateStreamFromJsonString(configString),
                    $"{modsetName}-config.json",
                    $"Config for {modsetName} is too long to fit in message. Please see attached file.");
                return;
            }

            await ReplyAsync($"Config for {modsetName}:\n```json\n{configString}\n```");
        }
        
        private static Stream CreateStreamFromJsonString(string jsonString)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);

            return new MemoryStream(bytes);
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
