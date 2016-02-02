using System;

namespace Win2ch.Common {
    public static class Extensions {
        public static T Apply<T>(this T item, Action<T> action) {
            action(item);
            return item;
        }

        public static bool EqualsNc(this string first, string second) {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }
    }
}
