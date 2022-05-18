using ArmaforcesMissionBot.Handlers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Attributes;

namespace ArmaforcesMissionBot.Modules
{
    [Name("Misc")]
    public class Misc : ModuleBase<SocketCommandContext>
    {
        public IServiceProvider _map { get; set; }
        public CommandService _commands { get; set; }

        public Misc()
        {
            //_map = map;
        }

        [Command("snipe")]
        [Summary("Displays recently deleted messages on the channel.")]
        //[RequireRank(RanksEnum.Recruiter)]
        public async Task Snipe(int count = 1)
        {
            count = Math.Min(count, 5);
            foreach (var message in MessageHandler._cachedDeletedMessages[Context.Channel.Id].Take(count))
            {
                var embed = new EmbedBuilder()
                        .WithColor(Color.Red)
                        .WithAuthor(message.Author)
                        .WithDescription(message.Content)
                        .WithTimestamp(message.CreatedAt);
                if (message.Attachments.Any())
                {
                    MemoryStream stream = new MemoryStream();
                    stream.Write(MessageHandler._cachedImages[message.Id], 0, MessageHandler._cachedImages[message.Id].Length);
                    stream.Position = 0;
                    embed.WithImageUrl($"attachment://{message.Attachments.First().Filename}");
                    await Context.Channel.SendFileAsync(stream, message.Attachments.First().Filename, embed: embed.Build());
                }
                else
                    await Context.Channel.SendMessageAsync("", embed: embed.Build());
            }
        }

        [Command("editsnipe")]
        //[Summary("Displays recently edited messages on the channel")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task EditSnipe(int count = 1)
        {
            count = Math.Min(count, 5);
            foreach (var message in MessageHandler._cachedEditedMessages[Context.Channel.Id].Take(count))
            {
                var embed = new EmbedBuilder()
                        .WithColor(Color.Red)
                        .WithAuthor(message.Author)
                        .WithDescription(message.Content)
                        .WithTimestamp(message.CreatedAt);
                await Context.Channel.SendMessageAsync("", embed: embed.Build());
            }
        }

        [Command("help")]
        [Summary("Displays this message.")]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("Available commands:");

            foreach (var module in _commands.Modules)
            {
                string description = "";
                foreach (var command in module.Commands)
                {
                    if ((await command.CheckPreconditionsAsync(Context, _map)).IsSuccess)
                    {
                        var addition = $"**BIA!";
                        addition += $"{command.Name}** - {command.Summary}\n";
                        if (description.Length + addition.Length > 1024)
                        {
                            embed.AddField(module.Name, description);
                            description = "";
                        }
                        description += addition;
                    }
                }

                if (description != "")
                    embed.AddField(module.Name, description);
            }

            await ReplyAsync(embed: embed.Build());
        }
    }
}
