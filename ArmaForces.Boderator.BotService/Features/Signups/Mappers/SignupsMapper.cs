using System.Collections.Generic;
using System.Linq;
using ArmaForces.Boderator.BotService.Features.Missions.Mappers;
using ArmaForces.Boderator.BotService.Features.Signups.DTOs;
using ArmaForces.Boderator.Core.Signups.Models;

namespace ArmaForces.Boderator.BotService.Features.Signups.Mappers;

public static class SignupsMapper
{
    public static SignupDto Map(Signup signup)
        => new()
        {
            SignupId = signup.SignupsId,
            StartDate = signup.StartDate,
            CloseDate = signup.CloseDate,
            Mission = MissionMapper.Map(signup.Mission),
            Teams = Map(signup.Teams)
        };

    public static TeamDto Map(Team team)
        => new()
        {
            Name = team.Name,
            Slots = Map(team.Slots),
            Vehicle = team.Vehicle,
            RequiredDlcs = team.RequiredDlcs
        };

    public static List<TeamDto> Map(IEnumerable<Team> teams)
        => teams.Select(Map).ToList();

    public static SlotDto Map(Slot slot)
        => new()
        {
            SlotId = slot.SlotId,
            Name = slot.Name,
            Occupant = slot.Occupant,
            Vehicle = slot.Vehicle,
            RequiredDlcs = slot.RequiredDlcs
        };

    public static List<SlotDto> Map(IEnumerable<Slot> slots)
        => slots.Select(Map).ToList();
}
