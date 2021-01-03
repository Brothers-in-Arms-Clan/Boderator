using ArmaforcesMissionBot.Attributes;
using ArmaforcesMissionBot.DataClasses;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Exceptions;
using ArmaforcesMissionBot.Extensions;
using ArmaforcesMissionBot.Features;
using ArmaforcesMissionBot.Features.Signups.Importer;
using ArmaforcesMissionBot.Helpers;
using ArmaforcesMissionBotSharedClasses;
using static ArmaforcesMissionBot.DataClasses.OpenedDialogs;

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
        public SignupHelper SignupHelper { get; set; }
        public MiscHelper _miscHelper { get; set; }

        [Command("importuj-zapisy")]
        [Summary("Importuje zapisy z załączonego pliku *.txt. lub z wiadomości (preferując plik txt jeżeli obie rzeczy są). " +
                 "Czyta plik/wiadomość linia po linii, dołączając linie bez prefixu 'AF!' do poprzedniej komendy " +
                 "a następnie wywołuje komendy w kolejności. " +
                 "Ignoruje linie zaczynające się od '#' oraz '//' umożliwiając komentarze.")]
        [ContextDMOrChannel]
        public async Task ImportSignups([Remainder]string missionContent = null) {
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
        public async Task StartSignups([Remainder]string title)
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => 
                (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New  || 
                    x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started) && 
                x.Owner == Context.User.Id))
                await ReplyAsync("O ty luju, najpierw dokończ definiowanie poprzednich zapisów!");
            else
            {
                if (_client.GetGuild(_config.AFGuild).GetUser(Context.User.Id).Roles.Any(x => x.Id == _config.MissionMakerRole))
                {
                    var mission = new ArmaforcesMissionBotSharedClasses.Mission();

                    mission.Title = title;
                    mission.Owner = Context.User.Id;
                    mission.Date = DateTime.Now;
                    mission.Editing = ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New;

                    signups.Missions.Add(mission);


                    await ReplyAsync("Zdefiniuj reszte misji.");
                }
                else
                    await ReplyAsync("Luju ty, nie jestes uprawniony do tworzenia misji!");
            }
        }

        [Command("opis")]
        [Summary("Definicja opisu misji, dodając obrazek dodajesz obrazek do wołania misji.")]
        [ContextDMOrChannel]
        public async Task Description([Remainder]string description)
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x =>
                (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New ||
                    x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started) && 
                x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x =>
                (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New ||
                    x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started) && 
                x.Owner == Context.User.Id);

                mission.Description = description;

                if (Context.Message.Attachments.Count > 0)
                {
                    mission.Attachment = Context.Message.Attachments.ElementAt(0).Url;
                }

                await ReplyAsync("Teraz podaj nazwe modlisty.");
            }
            else
            {
                await ReplyAsync("Najpierw zdefiniuj nazwę misji cymbale.");
            }
        }

        [Command("modlista")]
        [Summary("Nazwa modlisty lub link do niej.")]
        [ContextDMOrChannel]
        public async Task Modlist([Remainder]string modlist)
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x =>
                (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New ||
                    x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started) &&
                x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x => 
                    (x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New ||
                        x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started) && 
                    x.Owner == Context.User.Id);

                var request = WebRequest.Create($"https://server.armaforces.com:8888/modsets/{modlist.Split('/').Last()}.csv");
                //request.Method = "HEAD";
                try
                {
                    WebResponse response = request.GetResponse();
                    mission.Modlist = $"https://modlist.armaforces.com/#/download/{modlist.Split('/').Last()}";

                    await ReplyAsync("Teraz podaj datę misji.");
                }
                catch(Exception e)
                {
                    await ReplyAsync("Ten link lub nazwa modlisty nie jest prawidłowy dzbanie!");
                }
            }
            else
            {
                await ReplyAsync("Najpierw zdefiniuj nazwę misji cymbale.");
            }
        }

        [Command("data")]
        [Summary("Definicja daty rozpoczęcia misji w formacie RRRR-MM-DD GG:MM.")]
        [ContextDMOrChannel]
        public async Task Date([Remainder] DateTime date) {
            if (date.IsInPast())
                await ReplyAsync(":warning: Podana data jest w przeszłości!");
            else if (date.IsNoLaterThanDays(1)) await ReplyAsync(":warning: Podana data jest za mniej niż 24 godziny!");

            var signups = _map.GetService<SignupsData>();

            var mission = signups.Missions.SingleOrDefault(
                x => (x.Editing == Mission.EditEnum.New || x.Editing == Mission.EditEnum.Started)
                                 && x.Owner == Context.User.Id);

            if (mission is null) {
                await ReplyAsync(":warning: Nie tworzysz ani nie edytujesz teraz żadnej misji.");
                return;
            }

            mission.Date = date;
            if (!mission.CustomClose)
                mission.CloseTime = date.AddMinutes(-60);

            await ReplyAsync($"Data misji ustawiona na {date}, za {date.FromNow()}.");
        }

        [Command("zamkniecie")]
        [Summary("Definiowanie czasu kiedy powinny zamknąć się zapisy, tak jak data w formacie RRRR-MM-DD GG:MM.")]
        [ContextDMOrChannel]
        public async Task Close([Remainder] DateTime closeDate) {
            if (closeDate.IsInPast())
                await ReplyAsync(":warning: Podana data jest w przeszłości!");
            else if (closeDate.IsNoLaterThanDays(1)) await ReplyAsync(":warning: Podana data jest za mniej niż 24 godziny!");

            var signups = _map.GetService<SignupsData>();

            var mission = signups.Missions.SingleOrDefault(
                x => (x.Editing == Mission.EditEnum.New || x.Editing == Mission.EditEnum.Started)
                     && x.Owner == Context.User.Id);

            if (mission is null) {
                await ReplyAsync(":warning: Nie tworzysz ani nie edytujesz teraz żadnej misji.");
                return;
            }

            if (closeDate < mission.Date) {
                mission.CloseTime = closeDate;
                mission.CustomClose = true;
                await ReplyAsync($"Data zamknięcia zapisów ustawiona na {closeDate}, za {closeDate.FromNow()}!");
            } else {
                await ReplyAsync(":warning: Zamknięcie zapisów musi być przed datą misji!");
            }
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
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id);

                var slotTexts = teamText.Split("|");

                if (slotTexts.Length > 1)
                {
                    var team = new ArmaforcesMissionBotSharedClasses.Mission.Team();
                    team.Name = slotTexts[0];
                    team.Pattern = "";

                    foreach (var slotText in slotTexts)
                    {
                        MatchCollection matches = Helpers.MiscHelper.GetSlotMatchesFromText(slotText);
                        if (matches.Count == 0)
                            continue;

                        Match match = matches.First();

                        if(match.Success)
                        {
                            var slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(match.Groups[1].Value, int.Parse(match.Groups[2].Value.Substring(1, match.Groups[2].Value.Length - 2)));
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
                        _dialogs,
                        Context,
                        embed.Build(),
                        (Dialog dialog) =>
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
                        (Dialog dialog) =>
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
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id);
                // SL
                var team = new ArmaforcesMissionBotSharedClasses.Mission.Team();
                team.Name = teamName + " SL | <:wsciekly_zulu:426139721001992193> [1] | 🚑 [1]";
                var slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "Dowódca",
                    "<:wsciekly_zulu:426139721001992193>",
                    1);
                team.Slots.Add(slot);

                slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "Medyk",
                    "🚑",
                    1);
                team.Slots.Add(slot);
                team.Pattern = "<:wsciekly_zulu:426139721001992193> [1] | 🚑 [1]";
                mission.Teams.Add(team);

                // team 1
                team = new ArmaforcesMissionBotSharedClasses.Mission.Team();
                team.Name = teamName + " 1 | <:wsciekly_zulu:426139721001992193> [1] | 🚑 [1] | <:beton:437603383373987853> [" + (teamSize-2).ToString() + "]";
                slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "Dowódca",
                    "<:wsciekly_zulu:426139721001992193>",
                    1);
                team.Slots.Add(slot);

                slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "Medyk",
                    "🚑",
                    1);
                team.Slots.Add(slot);

                slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "BPP",
                    "<:beton:437603383373987853>",
                    teamSize - 2);
                team.Slots.Add(slot);
                team.Pattern = "<:wsciekly_zulu:426139721001992193> [1] | 🚑 [1] | <:beton:437603383373987853> [" + (teamSize - 2).ToString() + "]";
                mission.Teams.Add(team);

                // team 2
                team = new ArmaforcesMissionBotSharedClasses.Mission.Team();
                team.Name = teamName + " 2 | <:wsciekly_zulu:426139721001992193> [1] | 🚑 [1] | <:beton:437603383373987853> [" + (teamSize - 2).ToString() + "]";
                slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "Dowódca",
                    "<:wsciekly_zulu:426139721001992193>",
                    1);
                team.Slots.Add(slot);

                slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "Medyk",
                    "🚑",
                    1);
                team.Slots.Add(slot);

                slot = new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
                    "BPP",
                    "<:beton:437603383373987853>",
                    teamSize - 2);
                team.Slots.Add(slot);
                team.Pattern = "<:wsciekly_zulu:426139721001992193> [1] | 🚑 [1] | <:beton:437603383373987853> [" + (teamSize - 2).ToString() + "]";
                mission.Teams.Add(team);

                await ReplyAsync("Jeszcze coś?");
            }
            else
            {
                await ReplyAsync("A może byś mi najpierw powiedział do jakiej misji chcesz dodać ten zespół?");
            }
        }

        [Command("dodaj-rezerwe")]
        [Summary(
	        "Dodaje rezerwę o nieograniczonej liczbie miejsc, przy podaniu w parametrze liczby udostępnia taką liczbę miejsc na kanale dla rekrutów z możliwością zapisu dla nich.")]
        [ContextDMOrChannel]
        public async Task AddReserve(int slots = 0)
        {
	        var signups = _map.GetService<SignupsData>();

	        if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id))
	        {
		        var mission = signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id);
		        // SL
		        var team = new ArmaforcesMissionBotSharedClasses.Mission.Team();
                team.Slots.Add(new ArmaforcesMissionBotSharedClasses.Mission.Team.Slot(
	                "Rezerwa",
                    "🚑",
	                slots));
                team.Pattern = $"Rezerwa 🚑 [{slots}]";
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
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id);

                var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("Sekcje:")
                .WithDescription(Helpers.MiscHelper.BuildEditTeamsPanel(mission.Teams, mission.HighlightedTeam));

                var message = await Context.Channel.SendMessageAsync(embed: embed.Build());
                mission.EditTeamsMessage = message.Id;
                mission.HighlightedTeam = 0;

                var reactions = new IEmote[5];
                reactions[0] = new Emoji("⬆");
                reactions[1] = new Emoji("⬇");
                reactions[2] = new Emoji("📍");
                reactions[3] = new Emoji("✂");
                reactions[4] = new Emoji("🔒");

                await message.AddReactionsAsync(reactions);
            }
        }

        [Command("przelacz-wolanie")]
        [Summary("Pozwala włączyć/wyłączyć wołanie wszystkich do zapisów.")]
        [ContextDMOrChannel]
        public async Task ToggleMentionEveryone()
        {
            var signups = _map.GetService<SignupsData>();

            var mission = signups.Missions.SingleOrDefault(
                x => (x.Editing == Mission.EditEnum.New || x.Editing == Mission.EditEnum.Started)
                     && x.Owner == Context.User.Id);

            if (mission is null)
            {
                await ReplyAsync(":warning: Nie tworzysz ani nie edytujesz teraz żadnej misji.");
                return;
            }

            mission.MentionEveryone = !mission.MentionEveryone;
            if (mission.MentionEveryone)
            {
                await ReplyAsync($"Wołanie wszystkich zostało włączone.");
            }
            else
            {
                await ReplyAsync($"Wołanie wszystkich zostało wyłączone.");
            }
        }

        [Command("koniec")]
        [Summary("Wyświetla dialog z potwierdzeniem zebranych informacji o misji.")]
        [ContextDMOrChannel]
        public async Task EndSignups()
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id);
                if (Helpers.SignupHelper.CheckMissionComplete(mission))
                {
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
                        _dialogs,
                       Context,
                       embed.Build(),
                       (Dialog dialog) =>
                       {
                           _dialogs.Dialogs.Remove(dialog);
                           _ = SignupHelper.CreateSignupChannel(signups, Context.User.Id, Context.Channel);
                           ReplyAsync("No to lecim!");
                       },
                       (Dialog dialog) =>
                       {
                           Context.Channel.DeleteMessageAsync(dialog.DialogID);
                           _dialogs.Dialogs.Remove(dialog);
                           ReplyAsync("Poprawiaj to szybko!");
                       });
                }
                else
                {
                    await ReplyAsync("Nie uzupełniłeś wszystkich informacji ciołku!");
                }
            }
            else
            {
                await ReplyAsync("Co ty chcesz kończyć jak nic nie zacząłeś?");
            }
        }

        [Command("zaladowane")]
        [Summary("Pokazuje załadowane misje do których odbywają się zapisy, opcja czysto debugowa.")]
        [ContextDMOrChannel]
        public async Task Loaded()
        {
            var signups = _map.GetService<SignupsData>();

            foreach(var mission in signups.Missions)
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
        [Summary("Anuluje tworzenie misji, usuwa wszystkie zdefiniowane o niej informacje. Nie anuluje to już stworzonych zapisów.")]
        [ContextDMOrChannel]
        public async Task CancelSignups()
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id))
            {
                signups.Missions.Remove(signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.New && x.Owner == Context.User.Id));

                await ReplyAsync("I tak nikt nie chce grać na twoich misjach.");
            }
            else
                await ReplyAsync("Siebie anuluj, nie tworzysz żadnej misji aktualnie.");
        }

        [Command("aktualne-misje")]
        [Summary("Wyświetla aktualnie przeprowadzane zapisy użytkownika wraz z indeksami.")]
        [ContextDMOrChannel]
        public async Task ListMissions()
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Owner == Context.User.Id && x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.NotEditing))
            {
                var mainEmbed = new EmbedBuilder()
                            .WithColor(Color.Orange);

                int index = 0;

                foreach (var mission in signups.Missions.Where(x => x.Owner == Context.User.Id && x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.NotEditing))
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

        [Command("anuluj-misje")]
        [Summary("Po podaniu indeksu misji jako parametru anuluje całe zapisy usuwając kanał zapisów.")]
        [ContextDMOrChannel]
        public async Task CancelMission(int missionNo)
        {
            var signups = _map.GetService<SignupsData>();

            int index = 0;

            foreach (var mission in signups.Missions.Where(x => x.Owner == Context.User.Id && x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.NotEditing))
            {
                if (index++ == missionNo)
                {
                    await mission.Access.WaitAsync(-1);
                    try
                    {
                        var guild = _client.GetGuild(_config.AFGuild);
                        await guild.GetTextChannel(mission.SignupChannel).DeleteAsync();
                    }
                    finally
                    {
                        mission.Access.Release();
                    }
                }
            }

            await ReplyAsync("I tak by sie zjebała.");
        }

        [Command("edytuj-misje")]
        [Summary("Po podaniu indeksu misji jako parametru włączy edycje danej misji (część bez zespołów).")]
        [ContextDMOrChannel]
        public async Task EditMission(int missionNo)
        {
            var signups = _map.GetService<SignupsData>();

            int index = 0;

            foreach (var mission in signups.Missions.Where(x => x.Owner == Context.User.Id && x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.NotEditing))
            {
                if (index++ == missionNo)
                {
                    // Don't want to write another function just to copy class, and performance isn't a problem here so just serialize it and deserialize
                    var serialized = JsonConvert.SerializeObject(mission);
                    signups.BeforeEditMissions[Context.User.Id] = JsonConvert.DeserializeObject<ArmaforcesMissionBotSharedClasses.Mission>(serialized);
                    mission.Editing = ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started;
                    await ReplyAsync("Luzik, co chcesz zmienić?");
                }
            }
        }

        [Command("zapisz-zmiany")]
        [Summary("Zapisuje zmiany w aktualnie edytowanej misji, jesli w parametrze zostanie podana wartość true to zostanie wysłane ogłoszenie o zmianach w misji.")]
        [ContextDMOrChannel]
        public async Task SaveChanges(bool announce = false)
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started && x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started && x.Owner == Context.User.Id);

                await mission.Access.WaitAsync(-1);
                try
                {
                    if (Helpers.SignupHelper.CheckMissionComplete(mission))
                    {
                        var guild = _client.GetGuild(_config.AFGuild);

                        var channel = await SignupHelper.UpdateMission(guild, mission, signups);

                        mission.Editing = ArmaforcesMissionBotSharedClasses.Mission.EditEnum.NotEditing;

                        if(announce)
                            await channel.SendMessageAsync("@everyone Misja uległa modyfikacji, proszę zapoznać się z nowymi informacjami i dostosować swój beton.");

                        await ReplyAsync("Się robi szefie!");
                    }
                    else
                    {
                        await ReplyAsync("Nie uzupełniłeś wszystkich informacji ciołku!");
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Oj, coś poszło nie tak: {e.Message}");
                }
                finally
                {
                    mission.Access.Release();
                }
            }
        }

        [Command("anuluj-edycje")]
        [Summary("Anuluje aktualną edycję misji bez zapisywania zmian.")]
        [ContextDMOrChannel]
        public async Task CancelChanges(bool announce = false)
        {
            var signups = _map.GetService<SignupsData>();

            if (signups.Missions.Any(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started && x.Owner == Context.User.Id))
            {
                var mission = signups.Missions.Single(x => x.Editing == ArmaforcesMissionBotSharedClasses.Mission.EditEnum.Started && x.Owner == Context.User.Id);
                
                await mission.Access.WaitAsync(-1);
                try
                {
                    // Don't want to write another function just to copy class, and performance isn't a problem here so just serialize it and deserialize
                    signups.Missions.Remove(mission);
                    var serialized = JsonConvert.SerializeObject(signups.BeforeEditMissions[Context.User.Id]);
                    var oldMission = JsonConvert.DeserializeObject<ArmaforcesMissionBotSharedClasses.Mission>(serialized);
                    signups.Missions.Add(oldMission);

                    oldMission.Editing = ArmaforcesMissionBotSharedClasses.Mission.EditEnum.NotEditing;
                    await ReplyAsync("I dobrze, tylko byś ludzi wkurwiał...");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Oj, coś poszło nie tak: {e.Message}");
                }
                finally
                {
                    mission.Access.Release();
                }
            }
        }

        [Command("upgrade")]
        [Summary("Wykonuje potrzebne upgrade'y kanałów, może jej użyć tylko Ilddor.")]
        [RequireOwner]
        public async Task Upgrade()
        {
            var signups = _map.GetService<SignupsData>();

            foreach (var mission in signups.Missions)
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
                            var channel = await SignupHelper.UpdateMission(guild, mission, signups);
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

            await Helpers.BanHelper.MakeBanHistoryMessage(_map, Context.Guild);
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
