using System;

namespace Win2ch.Extensions {
    public static class CommonExtensions {
        public static T Apply<T>(this T obj, Action<T> func) {
            func?.Invoke(obj);
            return obj;
        }

        public static bool EqualsNc(this string first, string second) {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }
    }
}