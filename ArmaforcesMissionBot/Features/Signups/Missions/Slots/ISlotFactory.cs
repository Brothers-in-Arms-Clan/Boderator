namespace ArmaforcesMissionBot.Features.Signups.Missions.Slots
{
    internal interface ISlotFactory
    {
        Slot CreateSlot(
            string name,
            string emoji,
            int count);
    }
}
