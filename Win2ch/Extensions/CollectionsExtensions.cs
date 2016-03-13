using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Win2ch.Extensions {
    public static class CollectionsExtensions {
        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> items) {
            foreach (T item in items) {
                source.Add(item);
            }
        }
    }
}