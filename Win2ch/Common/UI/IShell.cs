using Win2ch.Common.Core;

namespace Win2ch.Common.UI {
    public interface IShell {
        LoadingInfo LoadingInfo { get; }
        void Navigate<T>(object parameter = null) where T : Tab;
        void ShowPopup(object content);
        void HidePopup();
    }
}