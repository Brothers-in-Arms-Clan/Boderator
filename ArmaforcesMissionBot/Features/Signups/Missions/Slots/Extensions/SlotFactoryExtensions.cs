namespace ArmaforcesMissionBot.Features.Signups.Missions.Slots.Extensions
{
    internal static class SlotFactoryExtensions
    {
        public static Slot CreateSlot(
            this ISlotFactory slotFactory,
            string emoji,
            int count)
            => slotFactory.CreateSlot(
                "",
                emoji,
                count);
    }
}
