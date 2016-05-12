using Windows.UI.Xaml;
using Template10.Services.SettingsService;
using Template10.Utils;
using TheChan.Common.UI;
using TheChan.Extensions;
using TheChan.Views;

namespace TheChan.Services.Settings {
    public class SettingsService : ISettingsService {
        public SettingsService(IShell shell) {
            Shell = shell;
        }

        private SettingsHelper Helper { get; } = new SettingsHelper();
        private IShell Shell { get; }

        public double FontScale {
            get { return Helper.Read(nameof(FontScale), 1.0); }
            set { Helper.Write(nameof(FontScale), value); }
        }

        public Theme CurrentTheme {
            get { return Helper.Read(nameof(CurrentTheme), Theme.System); }
            set {
                Helper.Write(nameof(CurrentTheme), value);
                ApplyTheme();
            }
        }

        private void ApplyTheme() {
            if (ShellView.Current != null)
                ShellView.Current.RequestedTheme = CurrentTheme == Theme.System
                    ? Application.Current.RequestedTheme.ToElementTheme()
                    : CurrentTheme.ToApplicationTheme().ToElementTheme();
        }
    }
}