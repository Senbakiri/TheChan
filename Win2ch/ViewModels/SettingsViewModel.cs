using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.UI.Xaml;
using Caliburn.Micro;
using Win2ch.Common;
using Win2ch.Common.UI;

namespace Win2ch.ViewModels {
    public class SettingsViewModel : PropertyChangedBase {
        private int currentLanguageIndex;
        private bool hasLanguageChanged;

        public SettingsViewModel(IShell shell) {
            Shell = shell;
            AvailableLanguages = new ObservableCollection<CultureInfo>(
                ApplicationLanguages.Languages.Select(l => new CultureInfo(l)));
            CurrentLanguage = CultureInfo.CurrentUICulture;
            this.currentLanguageIndex = AvailableLanguages.IndexOf(CurrentLanguage);
            Application.Current.Suspending += ApplyLanguage;
        }


        private IShell Shell { get; }
        private CultureInfo CurrentLanguage { get; set; }
        public ObservableCollection<CultureInfo> AvailableLanguages { get; }

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

        public bool HasLanguageChanged {
            get { return this.hasLanguageChanged; }
            private set {
                if (value == this.hasLanguageChanged)
                    return;
                this.hasLanguageChanged = value;
                NotifyOfPropertyChange();
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