using System;

namespace ArmaforcesMissionBot.Features.Signups.Missions
{
    public interface IMission
    {
        string Title { get; }

        DateTime Date { get; }

        DateTime? CloseTime { get; }

        string Description { get; }

        string Modlist { get; }

        ulong FreeSlots { get; }

        ulong AllSlots { get; }
    }
}
