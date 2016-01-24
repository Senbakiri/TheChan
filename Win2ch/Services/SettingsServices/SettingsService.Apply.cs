using System;
using Windows.UI.Xaml;

namespace Win2ch.Services.SettingsServices {
    public partial class SettingsService {
        public void ApplyUseShellBackButton(bool value) {
            Template10.Common.BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() => {
                Template10.Common.BootStrapper.Current.ShowShellBackButton = value;
                Template10.Common.BootStrapper.Current.UpdateShellBackButton();
                Template10.Common.BootStrapper.Current.NavigationService.Refresh();
            });
        }

        public void ApplyAppTheme(Theme value) {
            var theme = value != Theme.System
                ? (ApplicationTheme)value
                : Application.Current.RequestedTheme;

            Views.Shell.HamburgerMenu.RefreshStyles(theme);
        }

        private void ApplyCacheMaxDuration(TimeSpan value) {
            Template10.Common.BootStrapper.Current.CacheMaxDuration = value;
        }
    }
}

