using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Exceptions;
using ArmaforcesMissionBot.Extensions;
using ArmaforcesMissionBot.Features;
using ArmaforcesMissionBot.Features.Emojis.Constants;
using ArmaforcesMissionBot.Features.Signups;
using ArmaforcesMissionBot.Features.Signups.Importer;
using ArmaforcesMissionBot.Features.Signups.Missions;
using ArmaforcesMissionBot.Features.Signups.Missions.Slots;
using ArmaforcesMissionBot.Features.Signups.Missions.Slots.Extensions;
using ArmaforcesMissionBot.Helpers;
using CSharpFunctionalExtensions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace ArmaforcesMissionBot.Modules
{
    [Name("Zapisy")]
    public class Signups : ModuleBase<SocketCommandContext>, IModule
    {
        public IServiceProvider _map { get; set; }
        public DiscordSocketClient _client { get; set; }
        public Config _config { get; set; }
        public OpenedDialogs _dialogs { get; set; }
        public CommandService _commands { get; set; }
        public SignupsData SignupsData { get; set; }
        public ISlotFactory SlotFactory { get; set; }
        public SignupHelper SignupHelper { get; set; }
        public MiscHelper _miscHelper { get; set; }
        public ISignupsLogic SignupsLogic { get; set; }

        [Command("importuj-zapisy")]
        [Summary("Importuje zapisy z załączonego pliku *.txt. lub z wiadomości (preferując plik txt jeżeli obie rzeczy są). " +
                 "Czyta plik/wiadomość linia po linii, dołączając linie bez prefixu 'AF!' do poprzedniej komendy " +
                 "a następnie wywołuje komendy w kolejności. " +
                 "Ignoruje linie zaczynające się od '#' oraz '//' umożliwiając komentarze.")]
        [ContextDMOrChannel]
        public async Task ImportSignups([Remainder] string missionContent = null) {
            if (_client.GetGuild(_config.AFGuild)
                .GetUser(Context.User.Id)
                .Roles.All(x => x.Id != _config.MissionMakerRole))
                await ReplyWithException<NotAuthorizedException>("Nie jesteś uprawniony do tworzenia misji.");

            if (SignupsData.Missions.Any(
                x =>
                    (x.Editing == Mission.EditEnum.New ||
                     x.Editing == Mission.EditEnum.Started) &&
                    x.Owner == Context.User.Id))
                await ReplyWithException<MissionEditionInProgressException>(
                    "Edytujesz bądź tworzysz już misję!");


            if (Context.Message.Attachments.Any(x => x.Filename.Contains(".txt"))) {
                using var client = new HttpClient();
                var response = await client.GetAsync(Context.Message.Attachments.First().Url);
                missionContent = await response.Content.ReadAsStringAsync();
            }

            if (missionContent is null)
                await ReplyWithException<InvalidCommandParametersException>("Niepoprawne parametry komendy.");

            var signupImporter = new SignupImporter(Context, _commands, _map, this);

            await signupImporter.ProcessMessage(missionContent);
            
            await ReplyAsync("Zdefiniuj reszte misji.");
        }

        [Command("zrob-zapisy")]
        [Summary("Tworzy nową misję, jako parametr przyjmuje nazwę misji.")]
        [ContextDMOrChannel]
        public async Task StartSignups([Remainder] string title)
        {
            await SignupsLogic.StartSignupsCreation(Context.User, title).Match(
                onSuccess: message => ReplyAsync(message),
                onFailure: ReplyWithError);
        }

        [Command("opis")]
        [Summary("Definicja opisu misji, dodając obrazek dodajesz obrazek do wołania misji.")]
        [ContextDMOrChannel]
        public async Task Description([Remainder] string description)
        {
            await SignupsLogic.SetDescription(
                    Context.User,
                    description,
                    Context.Message.Attachments.FirstOrDefault())
                .Match(
                    onSuccess: message => ReplyAsync(message),
                    onFailure: ReplyWithError);
        }

        [Command("modlista")]
        [Summary("Nazwa modlisty lub link do niej.")]
        [ContextDMOrChannel]
        public async Task Modlist([Remainder] string modsetNameOrUrl)
        {
            await SignupsLogic.SetModset(Context.User, modsetNameOrUrl).Match(
                onSuccess: message => ReplyAsync(message),
                onFailure: ReplyWithError);
        }

        [Command("data")]
        [Summary("Definicja daty rozpoczęcia misji w formacie RRRR-MM-DD GG:MM. Można ją wymusić dopisując 'true' na końcu.")]
        [ContextDMOrChannel]
        public async Task Date(DateTime date, bool forceDate = false) {
            SignupsLogic.SetDate(Context.User, date, forceDate).Match(
                onSuccess: messages =>
                {
                    foreach (var message in messages)
                    {
                        ReplyAsync(message);
                    }
                },
                onFailure: async error => await ReplyWithError(error));
        }

        [Command("zamkniecie")]
        [Summary("Definiowanie czasu kiedy powinny zamknąć się zapisy, tak jak data w formacie RRRR-MM-DD GG:MM. " +
                 "Można ją wymusić dopisując 'true' na końcu.")]
        [ContextDMOrChannel]
        public async Task Close(DateTime closeDate, bool forceDate = false) {
            SignupsLogic.SetCloseDate(Context.User, closeDate, forceDate).Match(
                onSuccess: messages =>
                {
                    foreach (var message in messages)
                    {
                        ReplyAsync(message);
                    }
                },
                onFailure: async error => await ReplyWithError(error));
        }

        [Command("dodaj-sekcje", RunMode = RunMode.Async)]
        [Summary("Definiowanie sekcji w formacie `Nazwa | emotka [liczba] opcjonalna_nazwa_slota`, gdzie `Nazwa` to nazwa sekcji, " +
                 "emotka to emotka używana do zapisywania się na rolę, [liczba] to liczba miejsc w danej roli. " +
                 "Przykład `Zulu | :wsciekly_zulu: [1]` lub `Alpha 1 | :wsciekly_zulu: [1] Dowódca | 🚑 [1] Medyk | :beton: [5] BPP`" +
                 " może być podanych kilka różnych emotek. Kolejność dodawania " +
                 "sekcji pozostaje jako kolejność wyświetlania na zapisach. Prebeton odbywa się poprzez dopisanie na " +
                 "końcu roli osoby, która powinna być prebetonowana dla przykładu " +
                 "zabetonowany slot TL w standardowej sekcji będzie wyglądać tak `Alpha 1 | :wsciekly_zulu: [1] Dowódca @Ilddor#2556 | 🚑 [1] Medyk | :beton: [4] BPP`.")]
        [ContextDMOrChannel]
        public async Task AddTeam([Remainder]string teamText)
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);

                var slotTexts = teamText.Split("|");

                if (slotTexts.Length > 1)
                {
                    var team = new Team();
                    team.Name = slotTexts[0];
                    team.Pattern = "";

                    foreach (var slotText in slotTexts)
                    {
                        MatchCollection matches = MiscHelper.GetSlotMatchesFromText(slotText);
                        if (matches.Count == 0)
                            continue;

                        Match match = matches.First();

                        if(match.Success)
                        {
                            var slot = SlotFactory.CreateSlot(match.Groups[1].Value, int.Parse(match.Groups[2].Value.Substring(1, match.Groups[2].Value.Length - 2)));
                            if(match.Groups.Count == 4)
                            {
                                slot.Name = match.Groups[3].Value;
                                foreach(var user in Context.Message.MentionedUsers)
                                {
                                    if(slot.Name.Contains(user.Mention))
                                    {
                                        slot.Name = slot.Name.Replace(user.Mention, "");
                                        slot.Signed.Add(user.Id);
                                    }
                                }
                            }
                            team.Slots.Add(slot);

                            if (team.Pattern.Length > 0)
                                team.Pattern += "| ";
                            team.Pattern += $"{slot.Emoji} [{slot.Count}] {slot.Name} ";
                        }
                    }
                    
                    if (team.Slots
                        .GroupBy(x => x.Emoji)
                        .Any(x => x.Count() > 1))
                    {
                        await ReplyAsync("Zdublowałeś reakcje. Poprawiaj to!");
                        return;
                    }

                    var embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle(team.Name)
                        .WithDescription(_miscHelper.BuildTeamSlots(team)[0])
                        .WithFooter(team.Pattern);

                    _miscHelper.CreateConfirmationDialog(
                        Context,
                        embed.Build(),
                        dialog =>
                        {
                            Context.Channel.DeleteMessageAsync(dialog.DialogID);
                            _dialogs.Dialogs.Remove(dialog);
                            mission.Teams.Add(team);
                            foreach(var slot in team.Slots)
                            {
                                foreach(var signed in slot.Signed)
                                {
                                    mission.SignedUsers.Add(signed);
                                }
                            }
                            ReplyAsync("OK!");
                        }, 
                        dialog =>
                        {
                            Context.Channel.DeleteMessageAsync(dialog.DialogID);
                            _dialogs.Dialogs.Remove(dialog);
                            ReplyAsync("OK Boomer");
                        });
                }
            }
            else
            {
                await ReplyAsync("A może byś mi najpierw powiedział do jakiej misji chcesz dodać ten zespół?");
            }
        }

        [Command("dodaj-standardowa-druzyne")]
        [Summary("Definiuje druzyne o podanej nazwie (jeden wyraz) skladajaca sie z SL i dwóch sekcji, " +
                 "w kazdej sekcji jest dowódca, medyk i 4 bpp domyślnie. Liczbę bpp można zmienić podając " +
                 "jako drugi parametr sumaryczną liczbę osób na sekcję.")]
        [ContextDMOrChannel]
        public async Task AddTeam(string teamName, int teamSize = 6)
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);
                // SL
                mission.Teams.Add(
                    new Team
                    {
                        Name = teamName + $" SL | {EmoteConstants.WscieklyZulu} [1] | {EmojiConstants.Ambulance} [1]",
                        Slots = new List<Slot>
                        {
                            new Slot(
                                "Dowódca",
                                EmoteConstants.WscieklyZulu,
                                1),
                            new Slot(
                                "Medyk",
                                EmojiConstants.Ambulance,
                                1)
                        },
                        Pattern = $"{EmoteConstants.WscieklyZulu} [1] | {EmojiConstants.Ambulance} [1]"
                    });

                mission.Teams.Add(
                    new Team
                    {
                        Name = teamName +
                               $" 1 | {EmoteConstants.WscieklyZulu} [1] | {EmojiConstants.Ambulance} [1] | {EmoteConstants.Beton} [" +
                               (teamSize - 2) + "]",
                        Slots = new List<Slot>
                        {
                            new Slot(
                                "Dowódca",
                                EmoteConstants.WscieklyZulu,
                                1),
                            new Slot(
                                "Medyk",
                                EmojiConstants.Ambulance,
                                1),
                            new Slot(
                                "BPP",
                                EmoteConstants.Beton,
                                teamSize - 2)
                        },
                        Pattern =
                            $"{EmoteConstants.WscieklyZulu} [1] | {EmojiConstants.Ambulance} [1] | {EmoteConstants.Beton} [" +
                            (teamSize - 2) + "]"
                    });

                mission.Teams.Add(
                    new Team
                    {
                        Name = teamName +
                               $" 2 | {EmoteConstants.WscieklyZulu} [1] | {EmojiConstants.Ambulance} [1] | {EmoteConstants.Beton} [" +
                               (teamSize - 2) + "]",
                        Slots = new List<Slot>
                        {
                            new Slot(
                                "Dowódca",
                                EmoteConstants.WscieklyZulu,
                                1),
                            new Slot(
                                "Medyk",
                                EmojiConstants.Ambulance,
                                1),
                            new Slot(
                                "BPP",
                                EmoteConstants.Beton,
                                teamSize - 2)
                        },
                        Pattern =
                            $"{EmoteConstants.WscieklyZulu} [1] | {EmojiConstants.Ambulance} [1] | {EmoteConstants.Beton} [" +
                            (teamSize - 2) + "]"
                    });

                await ReplyAsync("Jeszcze coś?");
            }
            else
            {
                await ReplyWithError("A może byś mi najpierw powiedział do jakiej misji chcesz dodać ten zespół?");
            }
        }

        [Command("dodaj-rezerwe")]
        [Summary("Dodaje rezerwę o nieograniczonej liczbie miejsc, " +
                 "przy podaniu w parametrze liczby udostępnia taką liczbę " +
                 "miejsc na kanale dla rekrutów z możliwością zapisu dla nich.")]
        [ContextDMOrChannel]
        public async Task AddReserve(int slots = 0)
        {
	        if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
	        {
		        var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);
		        // SL
		        var team = new Team();
                team.Slots.Add(new Slot(
	                "Rezerwa",
                    EmojiConstants.Ambulance,
	                slots));
                team.Pattern = $"Rezerwa {EmojiConstants.Ambulance} [{slots}]";
                mission.Teams.Add(team);

                await ReplyAsync("Jeszcze coś?");
	        }
	        else
	        {
		        await ReplyAsync("A ta rezerwa to do czego?");
	        }
        }
        
        [Command("edytuj-sekcje")]
        [Summary("Wyświetla panel do ustawiania kolejnosci sekcji oraz usuwania. Strzałki przesuwają zaznaczenie/sekcje. " +
                 "Pinezka jest do \"złapania\" sekcji w celu przesunięcia. Nożyczki usuwają zaznaczoną sekcję. Kłódka kończy edycję sekcji.")]
        [ContextDMOrChannel]
        public async Task EditTeams()
        {
            if (SignupsData.Missions.Any(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = SignupsData.Missions.Single(x => x.Editing == Mission.EditEnum.New && x.Owner == Context.User.Id);

                var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("Sekcje:")
                .WithDescription(MiscHelper.BuildEditTeamsPanel(mission.Teams, mission.HighlightedTeam));

                var message = await Context.Channel.SendMessageAsync(embed: embed.Build());
                mission.EditTeamsMessage = message.Id;
                mission.HighlightedTeam = 0;

                var reactions = new List<IEmote>
                {
                    EmojiConstants.ArrowUpEmote,
                    EmojiConstants.ArrowDownEmote,
                    EmojiConstants.PinEmote,
                    EmojiConstants.ScissorsEmote,
                    EmojiConstants.LockEmote
                };

                await message.AddReactionsAsync(reactions.ToArray());
            }
        }

        [Command("przelacz-wolanie")]
        [Summary("Pozwala włączyć/wyłączyć wołanie wszystkich do zapisów.")]
        [ContextDMOrChannel]
        public async Task ToggleMentionEveryone()
        {
            await SignupsLogic.ToggleMentionEveryone(Context.User)
                .Match(
                    onSuccess: message => ReplyAsync(message),
                    onFailure: ReplyWithError);
        }

        [Command("koniec")]
        [Summary("Wyświetla dialog z potwierdzeniem zebranych informacji o misji.")]
        [ContextDMOrChannel]
        public async Task EndSignups()
        {
            var mission = SignupsData.GetCurrentlyCreatedMission(Context.User.Id);

            if (mission is null)
            {
                await ReplyAsync("Co ty chcesz kończyć jak nic nie zacząłeś?");
                return;
            }

            if (!SignupHelper.CheckMissionComplete(mission))
            {
                await ReplyAsync("Nie uzupełniłeś wszystkich informacji ciołku!");
                return;
            }

            var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle(mission.Title)
                .WithDescription(mission.Description)
                .WithFooter(mission.Date.ToString())
                .AddField("Zamknięcie zapisów:", mission.CloseTime.ToString())
                .AddField("Wołanie wszystkich:", mission.MentionEveryone)
                .WithAuthor(Context.User);

            if (mission.Attachment != null)
                embed.WithImageUrl(mission.Attachment);

            mission.Modlist ??= "https://modlist.armaforces.com/#/download/default";

            embed.AddField("Modlista:", mission.Modlist);

            _miscHelper.BuildTeamsEmbed(mission.Teams, embed);

            _miscHelper.CreateConfirmationDialog(
                Context,
                embed.Build(),
                dialog =>
                {
                    _dialogs.Dialogs.Remove(dialog);
                    _ = SignupHelper.CreateSignupChannel(
                        SignupsData,
                        Context.User.Id,
                        Context.Channel);
                    ReplyAsync("No to lecim!");
                },
                dialog =>
                {
                    Context.Channel.DeleteMessageAsync(dialog.DialogID);
                    _dialogs.Dialogs.Remove(dialog);
                    ReplyAsync("Poprawiaj to szybko!");
                });
        }

        [Command("zaladowane")]
        [Summary("Pokazuje załadowane misje do których odbywają się zapisy, opcja czysto debugowa.")]
        [ContextDMOrChannel]
        public async Task Loaded()
        {
            foreach(var mission in SignupsData.Missions)
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithTitle(mission.Title)
                    .WithDescription(mission.Description)
                    .WithFooter(mission.Date.ToString())
                    .AddField("Zamknięcie zapisów:", mission.CloseTime.ToString())
                    .WithAuthor(_client.GetUser(mission.Owner));

                if (mission.Attachment != null)
                    embed.WithImageUrl(mission.Attachment);

                if (mission.Modlist != null)
                    embed.AddField("Modlista:", mission.Modlist);
                else
                    embed.AddField("Modlista:", "Default");

                _miscHelper.BuildTeamsEmbed(mission.Teams, embed);

                var builtEmbed = embed.Build();

                await ReplyAsync($"{builtEmbed.Length}", embed: builtEmbed);
            }
        }

        [Command("anuluj")]
        [Summary(
            "Anuluje tworzenie misji, usuwa wszystkie zdefiniowane o niej informacje. Nie anuluje to już stworzonych zapisów.")]
        [ContextDMOrChannel]
        public async Task CancelSignups() => await CancelChanges();

        [Command("aktualne-misje")]
        [Summary("Wyświetla aktualnie przeprowadzane zapisy użytkownika wraz z indeksami.")]
        [ContextDMOrChannel]
        public async Task ListMissions()
        {
            if (SignupsData.Missions.Any(x => x.Owner == Context.User.Id && x.Editing == Mission.EditEnum.NotEditing))
            {
                var mainEmbed = new EmbedBuilder()
                            .WithColor(Color.Orange);

                int index = 0;

                foreach (var mission in SignupsData.Missions.Where(x => x.Owner == Context.User.Id && x.Editing == Mission.EditEnum.NotEditing))
                {
                    mainEmbed.AddField(index++.ToString(), mission.Title);
                }

                await ReplyAsync(embed: mainEmbed.Build());
            }
            else
            {
                await ReplyAsync("Jesteś leniem i nie masz żadnych aktualnie trwających zapisów na twoje misje.");
            }
        }

        [Command("odwolaj-misje")]
        [Summary("Po użyciu #wzmianki kanału misji jako parametru anuluje całe zapisy usuwając kanał zapisów.")]
        [ContextDMOrChannel]
        public async Task CancelMission(IGuildChannel channel)
        {
            var missionToBeCancelled = SignupsData.Missions.FirstOrDefault(x => x.SignupChannel == channel.Id);

            if (missionToBeCancelled == null)
            {
                await ReplyAsync("Nie ma misji o takiej nazwie.");
                return;
            }

            if (missionToBeCancelled.Owner != Context.User.Id)
            {
                await ReplyAsync("Nie nauczyli żeby nie ruszać nie swojego?");
                return;
            }

            await missionToBeCancelled.Access.WaitAsync(-1);
            try
            {
                var chanelToBeDeleted = await channel.Guild.GetTextChannelAsync(channel.Id);
                await chanelToBeDeleted.DeleteAsync();
                await ReplyAsync("I tak by sie zjebała.");
            }
            finally
            {
                missionToBeCancelled.Access.Release();
            }
        }

        [Command("edytuj-misje")]
        [Summary("Po użyciu #wzmianki kanału misji jako parametru włączy edycje danej misji (część bez zespołów).")]
        [ContextDMOrChannel]
        public async Task EditMission(IGuildChannel channel)
        {
            await SignupsLogic.StartSignupsEdition(Context.User, channel).Match(
                onSuccess: message => ReplyAsync(message),
                onFailure: ReplyWithError);
        }

        [Command("edytuj-nazwe-misji")]
        [Summary("Edycja nazwy już utworzonej misji.")]
        [ContextDMOrChannel]
        public async Task MissionName([Remainder] string newTitle)
        {
            await SignupsLogic.SetMissionName(Context.User, newTitle)
                .Match(
                    onSuccess: message => ReplyAsync(message),
                    onFailure: ReplyWithError);
        }

        [Command("zapisz-zmiany")]
        [Summary("Zapisuje zmiany w aktualnie edytowanej misji, jesli w parametrze zostanie podana wartość true to zostanie wysłane ogłoszenie o zmianach w misji.")]
        [ContextDMOrChannel]
        public async Task SaveChanges(bool announce = false)
        {
            await (await SignupsLogic.FinishSignupsEdition(Context.User))
                .Match(
                    onSuccess: message => ReplyAsync(message),
                    onFailure: ReplyWithError);
        }

        [Command("anuluj-edycje")]
        [Summary("Anuluje aktualną edycję misji bez zapisywania zmian.")]
        [ContextDMOrChannel]
        public async Task CancelChanges(bool announce = false)
        {
            await SignupsLogic.CancelSignupsEdition(Context.User)
                .Match(
                    onSuccess: message => ReplyAsync(message),
                    onFailure: ReplyWithError);
        }

        [Command("upgrade")]
        [Summary("Wykonuje potrzebne upgrade'y kanałów, może jej użyć tylko Ilddor.")]
        [RequireOwner]
        public async Task Upgrade()
        {
            foreach (var mission in SignupsData.Missions)
            {
                await mission.Access.WaitAsync(-1);
                try
                {
                    Uri uriResult;
                    bool validUrl = Uri.TryCreate(mission.Modlist, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if(!validUrl)
                    {
                        bool recheck = Uri.TryCreate($"https://modlist.armaforces.com/#/download/{mission.Modlist}", UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        if(recheck)
                        {
                            mission.Modlist = $"https://modlist.armaforces.com/#/download/{mission.Modlist}";
                            var guild = _client.GetGuild(_config.AFGuild);
                            var channel = await SignupHelper.UpdateMission(guild, mission, SignupsData);
                            await ReplyAsync($"Misja {mission.Title} zaktualizowana.");
                        }
                    }
                }
                finally
                {
                    mission.Access.Release();
                }
            }

            await ReplyAsync("No i cyk, gotowe.");

            await BanHelper.MakeBanHistoryMessage(_map, Context.Guild);
        }

        /// <summary>
        /// Replies to user with message and throws <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Exception type to throw</typeparam>
        /// <param name="message">Exception message</param>
        /// <returns>Throws <typeparamref name="T"/></returns>
        public async Task ReplyWithException<T>(string message = null) where T : Exception, new()
        {
            // ReSharper disable once PossibleNullReferenceException
            if (message != null)
            {
                await ReplyWithError(message);
                throw (T)Activator.CreateInstance(typeof(T), message);
            }

            var exception = new T();
            await ReplyWithError(exception.Message);
            throw exception;
        }

        private async Task ReplyWithError(string message)
        {
            await ReplyAsync(message ?? "Error when processing command.");
        }
    }
}
