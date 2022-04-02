using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Represents a mission.
/// </summary>
public record MissionDto
{
    /// <summary>
    /// Id of a mission.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public long MissionId { get; init; }

    /// <summary>
    /// Mission title.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Description for a mission.
    /// Must be provided before signups can be created.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; init; }
    
    /// <summary>
    /// Planned mission start date.
    /// Must be provided before signups can be created.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? MissionDate { get; init; }

    /// <summary>
    /// Name of modset for a mission.
    /// Must be provided before signups can be created.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? ModsetName { get; init; }

    /// <summary>
    /// Mission owner/organizer.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public string Owner { get; init; } = string.Empty;
        
    /// <summary>
    /// Signups for a mission.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public SignupsDto? Signups { get; init; }
}