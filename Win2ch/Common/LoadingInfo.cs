using Caliburn.Micro;

namespace Win2ch.Common {
    public class LoadingInfo : PropertyChangedBase {
        private bool isLoading;
        private string loadingText;

        public bool IsLoading {
            get { return isLoading; }
            set {
                if (value == isLoading)
                    return;
                isLoading = value;
                if (!value)
                    LoadingText = "";
                NotifyOfPropertyChange();
            }
        }

        public string LoadingText {
            get { return loadingText; }
            set {
                if (value == loadingText)
                    return;
                loadingText = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
