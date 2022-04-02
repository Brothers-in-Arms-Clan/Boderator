using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Player sign-up request for a slot.
/// </summary>
public record PlayerSignUpRequestDto
{
    /// <summary>
    /// Id of a slot.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public long SlotId { get; set; }

    /// <summary>
    /// Player name.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public string Player { get; set; } = string.Empty;
}