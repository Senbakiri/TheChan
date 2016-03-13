namespace Win2ch.Common {
    public interface IShell {
        void Navigate<T>(object parameter = null) where T : Tab;
    }
}