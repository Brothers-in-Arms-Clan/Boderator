using System.Collections.Generic;
using ArmaForces.Boderator.Core.Dlcs.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Represents a slot in game.
/// </summary>
public record SlotDto
{
    /// <summary>
    /// Id of a slot.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [SwaggerSchema(Nullable = true)]
    public long? SlotId { get; init; }
        
    /// <summary>
    /// Name of a slot.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// List of required DLCs to play on this slot.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [SwaggerSchema(Nullable = false)]
    public List<Dlc> RequiredDlcs { get; init; } = new();

    /// <summary>
    /// Optional vehicle information
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Vehicle { get; init; }
    
    /// <summary>
    /// Name of a player who occupies the slot.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Include)]
    public string? Occupant { get; init; }
}
