using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArmaforcesMissionBot.Modules
{
    [Name("Rangi")]
    public class Ranks : ModuleBase<SocketCommandContext>
    {
        public IServiceProvider _map { get; set; }
        public DiscordSocketClient _client { get; set; }
        public Config _config { get; set; }

        [Command("recruit")]
        [Summary("Assigns rank of recruit.")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task Recruit(IGuildUser user)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] {Context.User.Username} called recruit command");
            var signupRole = Context.Guild.GetRole(_config.SignupRole);
            if (user.RoleIds.Contains(_config.RecruitRole))
                await ReplyAsync($"But {user.Mention} has already been recruited.");
            else if (user.RoleIds.Contains(_config.SignupRole))
                await ReplyAsync($"But {user.Mention} is already in {signupRole.Mention}!");
            else
            {
                await user.AddRoleAsync(Context.Guild.GetRole(_config.RecruitRole));
                var recruitMessageText =
                    $"Congratiulation {user.Mention}! Welcome among the recruits. You have 3 weeks to play the mission and complete the preparatory training, " +
                    $"after that you will recieve the rank of #{signupRole.Name}#! Otherwise you will be removed from the discord. " +
                    $"With the possibility of returning in the future. " +
                    $"It is advised to check out this channel: {Context.Guild.GetTextChannel(_config.RecruitInfoChannel).Mention}. " +
                    $"If you have any questions, feel free to ask here: {Context.Guild.GetTextChannel(_config.RecruitAskChannel).Mention}.";
                var recruitMessage = await ReplyAsync(recruitMessageText);
                // Modify message to include rank mention but without mentioning it
                var replacedMessage = recruitMessage.Content;
                replacedMessage = Regex.Replace(replacedMessage, "#ArmaForces#", $"{signupRole.Mention}");
                await recruitMessage.ModifyAsync(x => x.Content = replacedMessage);
            }

            await Context.Message.DeleteAsync();
        }

        [Command("kick")]
        [Summary("Kick the person from the Discord Server")]
        [RequireRank(RanksEnum.Recruiter)]
        public async Task Kick(IGuildUser user)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] {Context.User.Username} called kick command");
            var signupRole = Context.Guild.GetRole(_config.SignupRole);
            var userRoleIds = user.RoleIds;
            if (userRoleIds.All(x => x == _config.RecruitRole || x == _config.AFGuild))
            {
                var embedBuilder = new EmbedBuilder()
                {
                    ImageUrl = _config.KickImageUrl
                };
                var replyMessage =
                    await ReplyAsync(
                        $"{user.Mention} was kicked by {Context.User.Mention}.",
                        embed: embedBuilder.Build());
                await user.KickAsync("AFK");
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    await replyMessage.ModifyAsync(x => x.Embed = null);
                });
            }
            else
            {
                await ReplyAsync($"You cannot kick {user.Mention}, he is not a recruit.");
            }

            await Context.Message.DeleteAsync();
        }
    }
}
