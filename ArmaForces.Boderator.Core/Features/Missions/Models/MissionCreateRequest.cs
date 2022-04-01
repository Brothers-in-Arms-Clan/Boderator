using System;

namespace ArmaForces.Boderator.Core.Missions.Models;

public record MissionCreateRequest
{
    private readonly string _title = null!;
    private readonly string _owner = null!;
        
    public string Title
    {
        get => _title;
        init => _title = ValidateStringNotEmpty(value, nameof(Title));
    }

    public string? Description { get; init; }
        
    public DateTime? MissionTime { get; init; }
        
    public string? ModsetName { get; init; }

    public string Owner
    {
        get => _owner;
        init => _owner = ValidateStringNotEmpty(value, nameof(Owner));
    }

    private static string ValidateStringNotEmpty(string? value, string propertyName)
        => !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentNullException(propertyName);
}