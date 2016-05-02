namespace TheChan.Common.UI {
    public interface ICanScrollToItem<in T> {
        void ScrollToItem(T item);
    }
}