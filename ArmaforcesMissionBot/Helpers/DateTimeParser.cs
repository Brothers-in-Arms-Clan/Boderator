using System;

namespace ArmaforcesMissionBot.Helpers {
    public static class DateTimeParser {
        public static DateTime? ParseOrNull(string stringDateTime) {
            var parseSuccessful = DateTime.TryParse(stringDateTime, out var result);
            return parseSuccessful
                ? result
                : (DateTime?) null;
        }
    }
}
