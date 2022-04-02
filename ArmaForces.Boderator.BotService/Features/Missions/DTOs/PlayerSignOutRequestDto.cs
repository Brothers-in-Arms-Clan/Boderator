using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Player sign out request from a slot or signups.
/// </summary>
public record PlayerSignOutRequestDto
{
    /// <summary>
    /// Optional id of a slot. If not provided, player will be signed out from all slots.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    [SwaggerSchema(Nullable = false)]
    public long SlotId { get; set; }

    /// <summary>
    /// Player name.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public string Player { get; set; } = string.Empty;
}
