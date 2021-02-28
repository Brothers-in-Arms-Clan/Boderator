namespace ArmaforcesMissionBot.Features.Signups.Missions.Slots
{
    public interface ISlotFactory
    {
        Slot CreateSlot(
            string name,
            string emoji,
            int count);
    }
}
