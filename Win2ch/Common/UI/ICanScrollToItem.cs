namespace Win2ch.Common.UI {
    public interface ICanScrollToItem<in T> {
        void ScrollToItem(T item);
    }
}