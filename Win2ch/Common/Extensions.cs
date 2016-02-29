using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;

namespace Win2ch.Common {
    public static class Extensions {
        public static T Apply<T>(this T item, Action<T> action) {
            action(item);
            return item;
        }

        public static bool EqualsNc(this string first, string second) {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        public static T GetDataContext<T>(this object root)  where T : class {
            return (root as FrameworkElement)?.DataContext as T;
        }

        public static IEnumerable<IEnumerable<T>> Pair<T>(this IEnumerable<T> source) {
            var a = source.Select((value, index) => new { Index = index, Value = value })
                  .GroupBy(x => x.Index / 2)
                  .Select(g => g.ToList())
                  .Select(g => g.Select(i => i.Value));
            return a;
        }
    }
}
