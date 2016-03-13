namespace Win2ch.Common {
    public interface IShell {
        LoadingInfo LoadingInfo { get; }
        void Navigate<T>(object parameter = null) where T : Tab;
    }
}