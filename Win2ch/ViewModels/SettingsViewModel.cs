using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Template10.Mvvm;
using Win2ch.Services.SettingsServices;
using ViewModelBase = Win2ch.Mvvm.ViewModelBase;

namespace Win2ch.ViewModels {
    public class SettingsViewModel : ViewModelBase {

        private ISettingsService _settingsService;
        private string _ReleaseNotes;

        public string Version { get; }

        public string ReleaseNotes {
            get { return _ReleaseNotes; }
            private set {
                _ReleaseNotes = value;
                RaisePropertyChanged();
            }
        }

        public List<Theme> AvailableThemes { get; } = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
        public List<RepliesViewMode> AvailableRepliesViewModes
            { get; } = Enum.GetValues(typeof(RepliesViewMode)).Cast<RepliesViewMode>().ToList();

        public bool ScrollToPostWithImageAfterViewingImage {
            get { return _settingsService.ScrollToPostWithImageAfterViewingImage; }
            set {
                if (value == _settingsService.ScrollToPostWithImageAfterViewingImage)
                    return;

                _settingsService.ScrollToPostWithImageAfterViewingImage = value;
                RaisePropertyChanged();
            }
        }

        public Theme SelectedTheme {
            get { return _settingsService.AppTheme; }
            set {
                _settingsService.AppTheme = value;
                RaisePropertyChanged();
            }
        }

        public RepliesViewMode SelectedRepliesViewMode {
            get { return _settingsService.RepliesViewMode; }
            set {
                _settingsService.RepliesViewMode = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel() {
            _settingsService = SettingsService.Instance;

            var id = Package.Current.Id;
            Version = $"v{id.Version.Major}.{id.Version.Minor}.{id.Version.Build}";
            LoadReleaseNotes();
        }

        private async void LoadReleaseNotes() {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ReleaseNotes.txt"));
            var stream = await file.OpenReadAsync();
            var reader = new StreamReader(stream.AsStreamForRead());
            ReleaseNotes = reader.ReadToEnd();
            FullFillUnfulfilledConsumables();
        }

        private async void FullFillUnfulfilledConsumables() {
            var donations = await CurrentApp.GetUnfulfilledConsumablesAsync();
            foreach (var donation in donations) {
                await CurrentApp.ReportConsumableFulfillmentAsync(donation.ProductId, donation.TransactionId);
            }
        }

        public async void Donate() {
            var result = await CurrentApp.RequestProductPurchaseAsync("donation1");
            switch (result.Status) {
                case ProductPurchaseStatus.Succeeded:
                    await new MessageDialog("Спасибо за поддержку!").ShowAsync();
                    await CurrentApp.ReportConsumableFulfillmentAsync("donation1", result.TransactionId);
                    break;
                case ProductPurchaseStatus.NotFulfilled:
                    FullFillUnfulfilledConsumables();
                    break;
            }
        }
    }
}
