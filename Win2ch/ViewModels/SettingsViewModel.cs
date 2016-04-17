using Caliburn.Micro;
using Win2ch.Common;

namespace Win2ch.ViewModels {
    public class SettingsViewModel : PropertyChangedBase {
        public SettingsViewModel(IShell shell) {
            Shell = shell;
        }

        private IShell Shell { get; }

        public void RunCloudflareAuthorization() {
            Shell.Navigate<CloudflareViewModel>();
        }
    }
}