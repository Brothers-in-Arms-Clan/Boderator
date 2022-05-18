using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ArmaforcesMissionBot.DataClasses;
using ArmaforcesMissionBot.Features.RichPresence;
using ArmaforcesMissionBot.Handlers;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace ArmaforcesMissionBot.Services
{
    internal class StartupService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly Config _config;
        private readonly GameStatusUpdater _gameStatusUpdater;
        private readonly IServiceProvider _serviceProvider;

        private readonly string _botToken;
        
        private bool _botStarted = false;
        private List<IInstallable> _handlers;

        public StartupService(
            DiscordSocketClient client,
            Config config,
            IServiceProvider serviceProvider,
            GameStatusUpdater gameStatusUpdater)
        {
            _client = client;
            _config = config;
            _serviceProvider = serviceProvider;
            _gameStatusUpdater = gameStatusUpdater;

            _botToken = config.DiscordToken;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.GuildAvailable += Load;
            _client.GuildAvailable += WelcomeAsync;

            await LoadModules();

            await _client.LoginAsync(TokenType.Bot, _botToken);
            await _client.StartAsync();

            _gameStatusUpdater.StartTimer();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            return;
        }

        private async Task LoadModules()
        {
            _handlers = new List<IInstallable>();
            foreach (var handler in Assembly.GetEntryAssembly().DefinedTypes)
            {
                if (!handler.ImplementedInterfaces.Contains(typeof(IInstallable))) continue;
                _handlers.Add((IInstallable)Activator.CreateInstance(handler));
                _ = _handlers.Last().Install(_serviceProvider);
            }
        }

        private async Task Load(SocketGuild guild)
        {
            if (guild.CategoryChannels.Any(x => x.Id == _config.SignupsCategory))
            {
                var signups = guild.CategoryChannels.Single(x => x.Id == _config.SignupsCategory).Channels.Where(x => x.Id != _config.SignupsArchive);
            }
        }

        private async Task WelcomeAsync(SocketGuild guild)
        {
            if (_botStarted) return;
            
            _botStarted = true;

            if (guild.GetChannel(_config.CreateMissionChannel) is SocketTextChannel signupsChannel) 
                await signupsChannel.SendMessageAsync("Bot is up! 🍆");
        }
    }
}
