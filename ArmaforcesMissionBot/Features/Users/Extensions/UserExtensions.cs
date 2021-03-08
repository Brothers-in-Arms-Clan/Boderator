using Discord;

namespace ArmaforcesMissionBot.Features.Users.Extensions
{
    public static class UserExtensions
    {
        public static bool IsCurrentUser(this IUser user, IDiscordClient client)
        {
            return user.Id == client.CurrentUser.Id;
        }
    }
}
