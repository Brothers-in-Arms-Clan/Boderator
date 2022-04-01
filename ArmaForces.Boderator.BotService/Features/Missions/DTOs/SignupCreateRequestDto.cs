using System;
using System.Collections.Generic;
using ArmaForces.Boderator.Core.Missions.Models;
using Newtonsoft.Json;

namespace ArmaForces.Boderator.BotService.Features.Missions.DTOs;

public class SignupCreateRequestDto
{
    /// <summary>
    /// Id of the mission for which the signup will be created.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public int? MissionId { get; set; }
        
    /// <summary>
    /// Mission for which the signups will be created.
    /// Mission will be created.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public MissionCreateRequestDto? Mission { get; set; }
        
    /// <summary>
    /// Desired status of signups.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public SignupStatus? SignupStatus { get; set; }

    /// <summary>
    /// Starting date of signup.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public DateTime? StartDate { get; set; }
        
    /// <summary>
    /// Closing date of signup.
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public DateTime? CloseDate { get; set; }

    /// <summary>
    /// Teams available in signup.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public List<Team> Teams { get; set; } = new List<Team>();
}