using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.UI.Xaml;
using Caliburn.Micro;
using TheChan.Common.UI;
using TheChan.Services.Settings;

namespace TheChan.ViewModels {
    public class SettingsViewModel : PropertyChangedBase {
        private int currentLanguageIndex;
        private bool hasLanguageChanged;
        private int currentThemeIndex;

        public SettingsViewModel(IShell shell, ISettingsService settingsService) {
            Shell = shell;
            SettingsService = settingsService;
            AvailableLanguages = new ObservableCollection<CultureInfo>(
                ApplicationLanguages.Languages.Select(l => new CultureInfo(l)));
            CurrentLanguage = CultureInfo.CurrentUICulture;
            this.currentLanguageIndex = AvailableLanguages.IndexOf(CurrentLanguage);
            Application.Current.Suspending += ApplyLanguage;

            FontScale = SettingsService.FontScale;

            AvailableThemes = new ObservableCollection<Theme>(Enum.GetValues(typeof(Theme)).OfType<Theme>());
            CurrentThemeIndex = AvailableThemes.IndexOf(SettingsService.CurrentTheme);
        }


        private IShell Shell { get; }
        private ISettingsService SettingsService { get; }
        private CultureInfo CurrentLanguage { get; set; }
        public ObservableCollection<CultureInfo> AvailableLanguages { get; }
        public ObservableCollection<Theme> AvailableThemes { get; } 

        public int CurrentLanguageIndex {
            get { return this.currentLanguageIndex; }
            set {
                if (value == this.currentLanguageIndex)
                    return;
                this.currentLanguageIndex = value;

                if (value != -1) {
                    HasLanguageChanged = true;
                    CurrentLanguage = AvailableLanguages[value];
                }

                NotifyOfPropertyChange();
            }
        }

        public int CurrentThemeIndex {
            get { return this.currentThemeIndex; }
            set {
                if (value == this.currentThemeIndex)
                    return;
                this.currentThemeIndex = value;

                if (value != -1)
                    SettingsService.CurrentTheme = AvailableThemes[value];

                NotifyOfPropertyChange();
            }
        }

        public bool HasLanguageChanged {
            get { return this.hasLanguageChanged; }
            private set {
                if (value == this.hasLanguageChanged)
                    return;
                this.hasLanguageChanged = value;
                NotifyOfPropertyChange();
            }
        }

        public double FontScale {
            get { return SettingsService.FontScale; }
            set {
                if (value.Equals(SettingsService.FontScale))
                    return;
                SettingsService.FontScale = value;
            }
        }

        public void RunCloudflareAuthorization() {
            Shell.Navigate<CloudflareViewModel>();
        }

        private void ApplyLanguage(object sender, SuspendingEventArgs suspendingEventArgs) {
            if (HasLanguageChanged)
                ApplicationLanguages.PrimaryLanguageOverride = CurrentLanguage.Name;
        }
    }
}