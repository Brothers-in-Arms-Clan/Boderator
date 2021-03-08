using CSharpFunctionalExtensions;
using Discord;

namespace ArmaforcesMissionBot.Features.Signups.Missions.Slots.Extensions
{
    public static class SlotExtensions
    {
        public static bool HasFreeSpace(this Slot slot)
            => slot.Count > slot.Signed.Count;

        public static bool IsUserSigned(this Slot slot, IUser user)
            => IsUserSigned(slot, user.Id);

        public static bool IsUserSigned(this Slot slot, ulong userId)
        {
            return slot.Signed.Contains(userId);
        }

        public static Result SignUser(this Slot slot, IUser user)
            => SignUser(slot, user.Id);

        public static Result SignUser(this Slot slot, ulong userId)
        {
            if (!slot.HasFreeSpace())
            {
                return Result.Failure("No free space to sign user to this slot.");
            }

            if (slot.IsUserSigned(userId))
            {
                return Result.Failure("This user is already signed to this slot.");
            }

            slot.Signed.Add(userId);
            return Result.Success();
        }

        public static Result UnsignUser(this Slot slot, IUser user)
            => UnsignUser(slot, user.Id);

        public static Result UnsignUser(this Slot slot, ulong userId)
        {
            if (slot.IsUserSigned(userId))
            {
                slot.Signed.Remove(userId);
            }

            return Result.Success();
        }
    }
}
