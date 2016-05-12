using System;
using Windows.UI.Xaml;
using TheChan.Services.Settings;

namespace TheChan.Extensions {
    public static class CommonExtensions {
        public static T Apply<T>(this T obj, Action<T> func) {
            func?.Invoke(obj);
            return obj;
        }

        public static bool EqualsNc(this string first, string second) {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        public static ApplicationTheme ToApplicationTheme(this Theme theme) {
            if (theme == Theme.System)
                return default(ApplicationTheme);
            return (ApplicationTheme) (int) theme - 1;
        }
    }
}