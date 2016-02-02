using System;
using System.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Win2ch.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;
using Template10.Common;

namespace Win2ch {
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App {
        readonly ISettingsService _settings;

        public App() {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync();
            InitializeComponent();
            SplashFactory = e => new Views.Splash(e);

            #region App settings

            _settings = SettingsService.Instance;
            if (_settings.AppTheme != Theme.System)
                RequestedTheme = (ApplicationTheme)_settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args) {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                await StatusBar.GetForCurrentView().HideAsync();
            // setup hamburger shell
            var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            Window.Current.Content = new Views.Shell(nav);
            NavigationService.Navigate(typeof(Views.MainPage));
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args) {
            // perform long-running load
            await Task.Delay(0);

            // navigate to first page
            NavigationService.Navigate(typeof(Views.MainPage));
        }

    }
}

