using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ArmaForces.Boderator.Core.Missions.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Represents a signups for a mission.
/// </summary>
public record SignupsDto
{
    /// <summary>
    /// Id of a signups.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public long SignupsId { get; init; }
    
    /// <summary>
    /// Current signups status.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public SignupsStatus Status { get; init; }
    
    /// <summary>
    /// Scheduled start time for signups.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? StartDate { get; init; }
    
    /// <summary>
    /// Scheduled close time for signups.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? CloseDate { get; init; }

    /// <summary>
    /// List of teams available for players to sign up.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public List<TeamDto> Teams { get; init; } = new();
    
    /// <summary>
    /// Id of a mission which the signups are for.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    [Required]
    public long MissionId { get; init; }
}