using System;

namespace ArmaForces.Boderator.Core.Missions.Models
{
    public record Mission
    {
        public int MissionId { get; init; }

        public string Title { get; init; } = string.Empty;

        public DateTime? MissionTime { get; init; }

        public string? ModsetName { get; init; } = string.Empty;

        public string Owner { get; init; } = string.Empty;
    }
}
