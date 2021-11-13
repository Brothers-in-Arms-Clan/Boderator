using System;

namespace ArmaForces.Boderator.Core.Missions.Models
{
    public record MissionCreateRequest
    {
        public string Title { get; init; } = string.Empty;

        public string? Description { get; init; }
        
        public DateTime? MissionTime { get; init; }
        
        public string? ModsetName { get; init; }

        public string Owner { get; init; } = string.Empty;
    }
}
