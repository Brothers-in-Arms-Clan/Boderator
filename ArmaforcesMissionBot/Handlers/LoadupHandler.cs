using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Features.Modsets;
using ArmaforcesMissionBot.Features.Modsets.Legacy;
using ArmaforcesMissionBot.Features.Signups.Missions;
using ArmaforcesMissionBot.Features.Signups.Missions.Slots;
using ArmaforcesMissionBot.Helpers;
using static ArmaforcesMissionBot.DataClasses.SignupsData;

namespace ArmaforcesMissionBot.Handlers
{
    public class LoadupHandler : IInstallable
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private Config _config;
        private ModsetProvider _newModsetProvider;
        private LegacyModsetProvider _legacyModsetProvider;
        private ISlotFactory _slotFactory;

        public async Task Install(IServiceProvider map)
        {
            _client = map.GetService<DiscordSocketClient>();
            _config = map.GetService<Config>();
            _newModsetProvider = new ModsetProvider(map.GetService<IModsetsApiClient>());
            _legacyModsetProvider = new LegacyModsetProvider();
            _slotFactory = map.GetService<ISlotFactory>();
            _services = map;
            // Hook the MessageReceived event into our command handler
            _client.GuildAvailable += Load;
        }

        private async Task Load(SocketGuild guild)
        {
            Console.WriteLine($"[{DateTime.Now}] Loading up from: {guild.Name}");

            await LoadMissions(guild);
            await LoadBans(guild);
            await LoadBanHistory(guild);
            await LoadMissionsArchive(guild);
        }

        private async Task LoadMissions(SocketGuild guild)
        {
            var signups = _services.GetService<SignupsData>();

            var channels = guild.CategoryChannels.Single(x => x.Id == _config.SignupsCategory);

            Console.WriteLine($"[{DateTime.Now.ToString()}] Loading missions");

            foreach (var channel in channels.Channels.Where(
                    x => x.Id != _config.SignupsArchive && x.Id != _config.CreateMissionChannel &&
                         x.Id != _config.HallOfShameChannel)
                .Reverse())
            {
                if (signups.Missions.Any(x => x.SignupChannel == channel.Id))
                    continue;
                var mission = new Mission();

                var textChannel = channel as SocketTextChannel;
                var messages = textChannel.GetMessagesAsync();
                var messagesNormal = new List<IMessage>();
                await messages.ForEachAsync(
                    async x =>
                    {
                        foreach (var it in x) messagesNormal.Add(it);
                    });

                mission.SignupChannel = channel.Id;

                foreach (var message in messagesNormal)
                {
                    if (message.Embeds.Count == 0)
                        continue;

                    if (message.Author.Id != _client.CurrentUser.Id)
                        continue;

                    var embed = message.Embeds.Single();
                    if (embed.Author == null)
                    {
                        var pattern = "";
                        if (embed.Footer.HasValue)
                            pattern = embed.Footer.Value.Text;
                        else
                            pattern = embed.Title;

                        var matches = MiscHelper.GetSlotMatchesFromText(pattern);

                        if (matches.Count > 0)
                        {
                            var team = new Team
                            {
                                Name = embed.Title
                            };
                            pattern = "";
                            foreach (var match in matches.Reverse())
                            {
                                var icon = match.Groups[1].Value;
                                if (icon[0] == ':')
                                {
                                    var emotes = _client.GetGuild(_config.AFGuild).Emotes;
                                    var foundEmote = emotes.Single(x => x.Name == icon.Substring(1, icon.Length - 2));
                                    var animated = foundEmote.Animated ? "a" : "";
                                    icon = $"<{animated}:{foundEmote.Name}:{foundEmote.Id}>";
                                    pattern = pattern.Replace(match.Groups[1].Value, icon);
                                }

                                var count = match.Groups[2].Value;
                                var name = match.Groups[3].Success ? match.Groups[3].Value : "";
                                var slot = _slotFactory.CreateSlot(
                                    name,
                                    icon,
                                    int.Parse(count.Substring(1, count.Length - 2)));

                                if (!embed.Footer.HasValue)
                                    team.Name = team.Name.Replace(match.Groups[0].Value, "");
                                pattern += $"{match.Groups[0]} ";
                                team.Slots.Add(slot);

                                Console.WriteLine($"New slot {slot.Emoji} [{slot.Count}] {slot.Name}");
                            }

                            team.Name = team.Name.Replace("|", "");

                            if (embed.Description != null)
                                try
                                {
                                    var signedPattern = @"(.+)(?:\(.*\))?-\<\@\!([0-9]+)\>";
                                    var signedMatches = Regex.Matches(
                                        embed.Description,
                                        signedPattern,
                                        RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                                    foreach (var match in signedMatches.Reverse())
                                    {
                                        var signedID = ulong.Parse(match.Groups[2].Value);
                                        mission.SignedUsers.Add(signedID);
                                        Console.WriteLine(
                                            $"{match.Groups[1].Value} : {match.Groups[2].Value} ({signedID})");
                                        team.Slots.Single(x => x.Emoji.Name == match.Groups[1].Value).Signed.Add(signedID);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Failed loading team {team.Name} : {e.Message}");
                                }

                            team.TeamMsg = message.Id;
                            team.Pattern = pattern;
                            mission.Teams.Add(team);
                        }
                    } else
                    {
                        mission.Title = embed.Title;
                        mission.Description = embed.Description;
                        mission.Owner = ulong.Parse(embed.Author.Value.Url.Substring(BotConstants.DISCORD_USER_URL_PREFIX.Length));
                        // Do I need author id again?
                        mission.Attachment = embed.Image.HasValue ? embed.Image.Value.Url : null;
                        foreach (var field in embed.Fields)
                            switch (field.Name)
                            {
                                case "Data:":
                                    mission.Date = DateTime.ParseExact(
                                        field.Value,
                                        "dd.MM.yyyy HH:mm:ss",
                                        CultureInfo.InvariantCulture);
                                    break;
                                case "Modlista:":
                                    mission.Modlist = mission.ModlistUrl = field.Value;
                                    mission.ModlistName = GetModsetNameFromUnknownUrl(mission.ModlistUrl);
                                    break;
                                case "Zamknięcie zapisów:":
                                    uint timeDifference;
                                    if (!uint.TryParse(field.Value, out timeDifference))
                                        mission.CloseTime = DateTime.ParseExact(
                                            field.Value,
                                            "dd.MM.yyyy HH:mm:ss",
                                            CultureInfo.InvariantCulture);
                                    else
                                        mission.CloseTime = mission.Date.AddMinutes(-timeDifference);
                                    break;
                            }
                    }
                }

                mission.Teams.Reverse(); // As teams were read backwards due to reading messages backwards

                signups.Missions.Add(mission);
            }

            // Sort channels by date
            signups.Missions.Sort((x, y) => { return x.Date.CompareTo(y.Date); });
        }

        private async Task LoadBans(SocketGuild guild)
        {
            var signups = _services.GetService<SignupsData>();

            Console.WriteLine($"[{DateTime.Now.ToString()}] Loading bans");

            var banChannel = guild.Channels.Single(x => x.Id == _config.HallOfShameChannel) as SocketTextChannel;
            var messages = banChannel.GetMessagesAsync();
            var messagesNormal = new List<IMessage>();
            await messages.ForEachAsync(
                async x =>
                {
                    foreach (var it in x) messagesNormal.Add(it);
                });

            foreach (var message in messagesNormal)
            {
                if (message.Embeds.Count == 1 && message.Content == "Bany na zapisy:" &&
                    message.Author.Id == _client.CurrentUser.Id)
                {
                    if (signups.SignupBans.Count > 0)
                        continue;
                    signups.SignupBansMessage = message.Id;

                    await signups.BanAccess.WaitAsync(-1);
                    try
                    {
                        if (message.Embeds.First().Description != null)
                        {
                            var banPattern = @"(\<\@\![0-9]+\>)-(.*)(?:$|\n)";
                            var banMatches = Regex.Matches(
                                message.Embeds.First().Description,
                                banPattern,
                                RegexOptions.IgnoreCase);
                            foreach (Match match in banMatches)
                                signups.SignupBans.Add(
                                    ulong.Parse(match.Groups[1].Value.Substring(3, match.Groups[1].Value.Length - 4)),
                                    DateTime.Parse(match.Groups[2].Value));
                        }
                    } finally
                    {
                        signups.BanAccess.Release();
                    }
                }

                if (message.Embeds.Count == 1 && message.Content == "Bany za spam reakcjami:" &&
                    message.Author.Id == _client.CurrentUser.Id)
                {
                    if (signups.SpamBans.Count > 0)
                        continue;
                    signups.SpamBansMessage = message.Id;

                    await signups.BanAccess.WaitAsync(-1);
                    try
                    {
                        if (message.Embeds.First().Description != null)
                        {
                            var banPattern = @"(\<\@\![0-9]+\>)-(.*)(?:$|\n)";
                            var banMatches = Regex.Matches(
                                message.Embeds.First().Description,
                                banPattern,
                                RegexOptions.IgnoreCase);
                            foreach (Match match in banMatches)
                                signups.SpamBans.Add(
                                    ulong.Parse(match.Groups[1].Value.Substring(3, match.Groups[1].Value.Length - 4)),
                                    DateTime.Parse(match.Groups[2].Value));
                        }
                    } finally
                    {
                        signups.BanAccess.Release();
                    }
                }
            }
        }

        private async Task LoadBanHistory(SocketGuild guild)
        {
            var signups = _services.GetService<SignupsData>();

            var channels = guild.CategoryChannels.Single(x => x.Id == _config.SignupsCategory);

            Console.WriteLine($"[{DateTime.Now.ToString()}] Loading ban history");
            // History of bans
            var shameChannel = guild.Channels.Single(x => x.Id == _config.HallOfShameChannel) as SocketTextChannel;
            var messages = shameChannel.GetMessagesAsync();
            var messagesNormal = new List<IMessage>();
            await messages.ForEachAsync(
                async x =>
                {
                    foreach (var it in x) messagesNormal.Add(it);
                });

            foreach (var message in messagesNormal)
            {
                if (message.Embeds.Count == 1 && message.Content == "Historia banów na zapisy:" &&
                    message.Author.Id == _client.CurrentUser.Id)
                {
                    if (signups.SignupBansHistory.Count > 0)
                        continue;
                    signups.SignupBansHistoryMessage = message.Id;

                    await signups.BanAccess.WaitAsync(-1);
                    try
                    {
                        if (message.Embeds.First().Description != null)
                        {
                            var banPattern = @"(\<\@\![0-9]+\>)-([0-9]+)-([0-9]+)(?:$|\n)";
                            var banMatches = Regex.Matches(
                                message.Embeds.First().Description,
                                banPattern,
                                RegexOptions.IgnoreCase);
                            foreach (Match match in banMatches)
                                signups.SignupBansHistory.Add(
                                    ulong.Parse(match.Groups[1].Value.Substring(3, match.Groups[1].Value.Length - 4)),
                                    new Tuple<uint, uint>(
                                        uint.Parse(match.Groups[2].Value),
                                        uint.Parse(match.Groups[3].Value)));
                            Console.WriteLine($"[{DateTime.Now.ToString()}] Loaded signup ban history");
                        }
                    } finally
                    {
                        signups.BanAccess.Release();
                    }
                }

                if (message.Embeds.Count == 1 && message.Content == "Historia banów za spam reakcjami:" &&
                    message.Author.Id == _client.CurrentUser.Id)
                {
                    if (signups.SpamBansHistory.Count > 0)
                        continue;
                    signups.SpamBansHistoryMessage = message.Id;

                    await signups.BanAccess.WaitAsync(-1);
                    try
                    {
                        if (message.Embeds.First().Description != null)
                        {
                            var banPattern = @"(\<\@\![0-9]+\>)-([0-9]+)-(.*)-(.*)(?:$|\n)";
                            var banMatches = Regex.Matches(
                                message.Embeds.First().Description,
                                banPattern,
                                RegexOptions.IgnoreCase);
                            foreach (Match match in banMatches)
                                signups.SpamBansHistory.Add(
                                    ulong.Parse(match.Groups[1].Value.Substring(3, match.Groups[1].Value.Length - 4)),
                                    new Tuple<uint, DateTime, BanType>(
                                        uint.Parse(match.Groups[2].Value),
                                        DateTime.Parse(match.Groups[3].Value),
                                        (BanType) Enum.Parse(typeof(BanType), match.Groups[4].Value)));
                            Console.WriteLine($"[{DateTime.Now.ToString()}] Loaded reaction spam ban history");
                        }
                    } finally
                    {
                        signups.BanAccess.Release();
                    }
                }
            }
        }

        private async Task LoadMissionsArchive(SocketGuild guild)
        {
            var archive = _services.GetService<MissionsArchiveData>();

            var channels = guild.CategoryChannels.Single(x => x.Id == _config.SignupsCategory);

            Console.WriteLine($"[{DateTime.Now.ToString()}] Loading mission history");
            archive.ArchiveMissions.Clear();

            // History of missions
            var archiveChannel = guild.Channels.Single(x => x.Id == _config.SignupsArchive) as SocketTextChannel;
            var messages = archiveChannel.GetMessagesAsync(10000);
            var messagesNormal = new List<IMessage>();
            await messages.ForEachAsync(
                async x =>
                {
                    foreach (var it in x) messagesNormal.Add(it);
                });

            foreach (var message in messagesNormal)
            {
                if (message.Embeds.Count == 0)
                    continue;

                //hardcoded previous bot instance ID
                if (message.Author.Id != _client.CurrentUser.Id && message.Author.Id != 598928706165276729)
                    continue;

                var embed = message.Embeds.Single();

                var newArchiveMission = new MissionsArchiveData.Mission();

                newArchiveMission.Title = embed.Title;
                DateTime date;
                if (!DateTime.TryParseExact(
                    embed.Footer.Value.Text,
                    "dd.MM.yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out date))
                {
                    Console.WriteLine($"Loading failed on mission date: {embed.Footer.Value.Text}");
                    continue;
                }

                newArchiveMission.Date = date;
                newArchiveMission.CloseTime = message.Timestamp.DateTime;
                newArchiveMission.Description = embed.Description;
                newArchiveMission.Attachment = embed.Image.HasValue ? embed.Image.Value.Url : null;

                ulong signedUsers = 0;
                ulong slots = 0;
                foreach (var field in embed.Fields)
                    switch (field.Name)
                    {
                        case "Zamknięcie zapisów:":
                        case "Data:":
                            break;
                        case "Modlista:":
                            newArchiveMission.Modlist = newArchiveMission.ModlistUrl = field.Value;
                            newArchiveMission.ModlistName = GetModsetNameFromUnknownUrl(newArchiveMission.ModlistUrl);
                            break;
                        default:
                            var signedPattern = @"(.+)(?:\(.*\))?-(\<\@\!([0-9]+)\>)?";
                            var signedMatches = Regex.Matches(
                                field.Value,
                                signedPattern,
                                RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                            foreach (var match in signedMatches.Reverse())
                            {
                                slots++;
                                if (match.Groups[2].Success)
                                    signedUsers++;
                            }

                            break;
                    }

                newArchiveMission.FreeSlots = slots - signedUsers;
                newArchiveMission.AllSlots = slots;

                archive.ArchiveMissions.Add(newArchiveMission);
            }

            // Sort channels by date
            archive.ArchiveMissions.Sort((x, y) => { return x.Date.CompareTo(y.Date); });

            Console.WriteLine($"[{DateTime.Now.ToString()}] Loaded {archive.ArchiveMissions.Count} archive missions");
        }

        private string GetModsetNameFromUnknownUrl(string unknownUrl)
        {
            return unknownUrl.Contains("modlist.armaforces.com")
                ? _legacyModsetProvider.GetModsetNameFromUrl(unknownUrl)
                : _newModsetProvider.GetModsetNameFromUrl(unknownUrl);
        }
    }
}
