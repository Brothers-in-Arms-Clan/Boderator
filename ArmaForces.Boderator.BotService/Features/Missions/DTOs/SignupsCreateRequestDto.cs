using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ArmaForces.Boderator.Core.Missions.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

/// <summary>
/// Signups create request.
/// </summary>
public class SignupsCreateRequestDto
{
    /// <summary>
    /// Id of the mission for which the signups will be created.
    /// It can't be specified if "mission" is specified.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    [SwaggerSchema(Nullable = false)]
    public int? MissionId { get; set; }
        
    /// <summary>
    /// Mission for which the signups will be created.
    /// Mission will be created before creating signups and must be ready for signups creation.
    /// It can't be specified if "missionId" is specified.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    [SwaggerSchema(Nullable = false)]
    public MissionCreateRequestDto? Mission { get; set; }

    /// <summary>
    /// Desired status of signups.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    [DefaultValue(SignupsStatus.Created)]
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
    [Required]
    public List<TeamDto> Teams { get; set; } = new();
}