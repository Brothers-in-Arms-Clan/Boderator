using System;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Mission creation request.
/// </summary>
public record MissionCreateRequestDto
{
    /// <summary>
    /// Mission title.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional mission description. It's required before signups for the mission can be opened.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }

    /// <summary>
    /// Mission start time. It's required before signups for the mission can be opened.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? MissionDate { get; set; }

    /// <summary>
    /// Name of the modset. It's required before signups for the mission can be opened.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? ModsetName { get; set; }

    /// <summary>
    /// Owner of the mission.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    public string Owner { get; set; } = string.Empty;
}