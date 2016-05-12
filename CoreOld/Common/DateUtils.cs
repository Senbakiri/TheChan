using System;

namespace Core.Common {
    public static class DateUtils {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime TimestampToDateTime(long timestamp) {
            return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
        }
    }
}