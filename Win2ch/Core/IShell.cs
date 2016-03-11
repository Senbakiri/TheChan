namespace Win2ch.Core {
    public interface IShell {
        void Navigate<T>(object parameter = null) where T : Tab;
    }
}