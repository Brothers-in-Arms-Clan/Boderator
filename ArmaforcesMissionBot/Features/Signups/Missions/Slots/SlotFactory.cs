using ArmaforcesMissionBot.Features.Emojis;

namespace ArmaforcesMissionBot.Features.Signups.Missions.Slots
{
    internal class SlotFactory : ISlotFactory
    {
        private readonly IEmoteProvider _emoteProvider;

        public SlotFactory(IEmoteProvider emoteProvider)
        {
            _emoteProvider = emoteProvider;
        }

        public Slot CreateSlot(
            string name,
            string emoji,
            int count)
        {
            return new Slot(
                name,
                _emoteProvider.GetEmoteFromString(emoji),
                count);
        }
    }
}
