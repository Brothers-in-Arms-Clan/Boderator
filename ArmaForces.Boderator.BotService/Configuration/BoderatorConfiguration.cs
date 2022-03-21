namespace ArmaForces.Boderator.BotService.Configuration
{
    internal record BoderatorConfiguration
    {
        public string ConnectionString { get; init; } = string.Empty;
        
        public string DiscordToken { get; init; } = string.Empty;
    }
}
