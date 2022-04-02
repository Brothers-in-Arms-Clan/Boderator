using System;
using System.Collections.Generic;
using ArmaForces.Boderator.Core.Missions.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Mission create 
/// </summary>
public class SignupsCreateRequestDto
{
    /// <summary>
    /// Id of the mission for which the signups will be created.
    /// It can't be specified if "Mission" is specified.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public int? MissionId { get; set; }
        
    /// <summary>
    /// Mission for which the signups will be created.
    /// Mission will be created before creating signups and must be ready for signups creation.
    /// It can't be specified if "MissionId" is specified.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    [SwaggerSchema(Nullable = true)]
    public MissionCreateRequestDto? Mission { get; set; }

    /// <summary>
    /// Desired status of signups.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public SignupsStatus SignupsStatus { get; set; } = SignupsStatus.Created;

    /// <summary>
    /// Starting date of signups.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public DateTime? StartDate { get; set; }
        
    /// <summary>
    /// Closing date of signups.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public DateTime? CloseDate { get; set; }

    /// <summary>
    /// Teams available in signups.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [SwaggerSchema(Nullable = false)]
    public List<TeamDto> Teams { get; set; } = new();
}