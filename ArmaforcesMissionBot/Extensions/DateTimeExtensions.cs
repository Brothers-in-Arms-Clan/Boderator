using System;

namespace ArmaforcesMissionBot.Extensions {
    public static class DateTimeExtensions {

        /// <summary>
        /// Checks if <see cref="DateTime"/> is in the past.
        /// </summary>
        public static bool IsInPast(this DateTime dateTime)
            => dateTime < DateTime.Now;

        /// <summary>
        /// Checks if <see cref="DateTime"/> is no later than + <paramref name="days"/>.
        /// </summary>
        public static bool IsNoLaterThanDays(this DateTime dateTime, int days)
            => dateTime < DateTime.Now.AddDays(days);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> between now and given <see cref="DateTime"/>.
        /// </summary>
        public static TimeSpan FromNow(this DateTime dateTime) 
            => dateTime - DateTime.Now;
    }
}
