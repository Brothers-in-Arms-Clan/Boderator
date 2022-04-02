using System.Collections.Generic;
using ArmaForces.Boderator.Core.Dlcs.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Represents a team in game.
/// </summary>
public record TeamDto
{
    /// <summary>
    /// Name of a team.
    /// Must be unique within signups.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// List of required DLCs to play in this team.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [SwaggerSchema(Nullable = false)]
    public List<Dlc> RequiredDlcs { get; init; } = new();

    /// <summary>
    /// Optional vehicle information.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Vehicle { get; init; }
    
    /// <summary>
    /// Slots within a team.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    public List<SlotDto> Slots { get; init; } = new();
}