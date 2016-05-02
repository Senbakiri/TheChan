using TheChan.Common.Core;

namespace TheChan.Common.UI {
    public interface IShell {
        LoadingInfo LoadingInfo { get; }
        void Navigate<T>(object parameter = null) where T : Tab;
        void ShowPopup(object content);
        void HidePopup();
    }
}