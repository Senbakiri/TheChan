namespace Win2ch.Common {
    public interface ICanScrollToItem<in T> {
        void ScrollToItem(T item);
    }
}