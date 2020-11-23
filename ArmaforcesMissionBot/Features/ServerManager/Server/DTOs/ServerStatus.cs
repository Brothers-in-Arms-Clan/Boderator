namespace ArmaforcesMissionBot.Features.ServerManager.Server.DTOs
{
    public class ServerStatus
    {
        public int? HeadlessClientsConnected { get; set; }

        public bool IsServerRunning { get; set; }

        public string ModsetName { get; set; }

        public int Port { get; set; }
    }
}
