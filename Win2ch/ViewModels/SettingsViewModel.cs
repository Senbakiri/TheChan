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
using Microsoft.ApplicationInsights;
using Template10.Mvvm;
using Win2ch.Common;
using Win2ch.Services.SettingsServices;
using Win2ch.Models;
using Win2ch.Services;
using ViewModelBase = Win2ch.Mvvm.ViewModelBase;

namespace Win2ch.ViewModels {
    public class SettingsViewModel : ViewModelBase {

        private ISettingsService _settingsService;
        private string _ReleaseNotes;
        private readonly TelemetryClient _telemetryClient = new TelemetryClient();
        private string _VideoCacheSize;
        private string _TotalCacheSize;
        private bool _CanClearCache = true;

        public string Version { get; }

        public string ReleaseNotes {
            get { return _ReleaseNotes; }
            private set {
                _ReleaseNotes = value;
                RaisePropertyChanged();
            }
        }

        public double MaxLinesInPostOnBoard {
            get { return _settingsService.MaxLinesInPostOnBoard; }
            set {
                var correctValue = (int) Math.Max(0, value);
                if (_settingsService.MaxLinesInPostOnBoard == correctValue)
                    return;
                _settingsService.MaxLinesInPostOnBoard = correctValue;
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
                if (_settingsService.AppTheme == value)
                    return;
                TryChangeCurrentTheme(value);
            }
        }

        private async void TryChangeCurrentTheme(Theme theme) {
            var dialog = new MessageDialog("Для смены темы необходимо перезапустить приложение",
                "Требуется перезапуск");
            dialog.Commands.Add(new UICommand("Выход", _ => {
                _settingsService.AppTheme = theme;
                Application.Current.Exit();
            }));
            dialog.Commands.Add(new UICommand("Отмена", _ => RaisePropertyChanged(() => SelectedTheme)));
            await dialog.ShowAsync();
        }

        public RepliesViewMode SelectedRepliesViewMode {
            get { return _settingsService.RepliesViewMode; }
            set {
                _settingsService.RepliesViewMode = value;
                RaisePropertyChanged();
            }
        }

        public string VideoCacheSize {
            get { return _VideoCacheSize; }
            private set {
                if (value == _VideoCacheSize)
                    return;
                _VideoCacheSize = value;
                RaisePropertyChanged();
            }
        }

        public string TotalCacheSize {
            get { return _TotalCacheSize; }
            private set {
                if (value == _TotalCacheSize)
                    return;
                _TotalCacheSize = value;
                RaisePropertyChanged();
            }
        }

        public bool CanClearCache {
            get { return _CanClearCache; }
            private set {
                if (value == _CanClearCache)
                    return;
                _CanClearCache = value;
                RaisePropertyChanged();
            }
        }

        public bool IsWebmEnabled {
            get { return _settingsService.IsWebmEnabled; }
            set {
                if (_settingsService.IsWebmEnabled == value)
                    return;
                _settingsService.IsWebmEnabled = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel() {
            _settingsService = SettingsService.Instance;

            var id = Package.Current.Id;
            Version = $"v{id.Version.Major}.{id.Version.Minor}.{id.Version.Build}";
            LoadReleaseNotes();
            FullFillUnfulfilledConsumables();
            ShowCacheSizes();
        }

        private async void LoadReleaseNotes() {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ReleaseNotes.txt"));
            var stream = await file.OpenReadAsync();
            var reader = new StreamReader(stream.AsStreamForRead());
            ReleaseNotes = reader.ReadToEnd();
        }

        private async void ShowCacheSizes() {
            var cache = CacheService.Instance;
            try {
                var videoSize = await cache.GetCacheItemsSize(CacheItemType.Video);
                var totalSize = await cache.GetTotalCacheSize();
                VideoCacheSize = Utils.ToFileSize(videoSize);
                TotalCacheSize = Utils.ToFileSize(totalSize);
            } catch {
                VideoCacheSize = TotalCacheSize = "Не удалось получить размер";
            }
        }

        public async void ClearCache() {
            CanClearCache = false;

            try {
                await CacheService.Instance.Clear();
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось очистить кэш");
            }

            CanClearCache = true;
            ShowCacheSizes();
        }

        private async void FullFillUnfulfilledConsumables() {
            try {
                var donations = await CurrentApp.GetUnfulfilledConsumablesAsync();
                foreach (var donation in donations) {
                    await CurrentApp.ReportConsumableFulfillmentAsync(donation.ProductId, donation.TransactionId);
                }
            } catch (Exception e) {
                await Task.Run(() => _telemetryClient.TrackException(e));
            }
        }

        public async void Donate() {
            try {
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
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Произошла ошибка при обращении к серверу покупки");
                await Task.Run(() => _telemetryClient.TrackException(e));
            }
        }
    }
}
