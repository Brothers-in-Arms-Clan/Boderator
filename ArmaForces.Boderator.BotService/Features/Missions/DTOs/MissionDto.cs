using System;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs
{
    public class MissionDto
    {
        public int MissionId { get; set; }

        public string Title { get; set; } = string.Empty;
        
        public DateTime MissionDate { get; set; }
    }
}
