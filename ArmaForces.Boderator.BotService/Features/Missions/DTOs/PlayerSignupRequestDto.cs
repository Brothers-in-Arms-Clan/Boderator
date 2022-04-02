using Newtonsoft.Json;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Player sign-up request for a slot.
/// </summary>
public record PlayerSignupRequestDto
{
    /// <summary>
    /// Id of a slot.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public long SlotId { get; set; }

    /// <summary>
    /// Player name.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string Player { get; set; } = string.Empty;
}
