using System;

namespace ArmaforcesMissionBot.Features.ServerManager.Mods.DTOs
{
    public class ModsUpdateRequest
    {
        public string ModsetName { get; set; }

        public DateTime? ScheduleAt { get; set; }
    }
}
