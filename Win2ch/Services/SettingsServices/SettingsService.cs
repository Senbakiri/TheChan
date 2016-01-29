using System;
using Windows.UI.Xaml;

namespace Win2ch.Services.SettingsServices {
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SettingsService
    public partial class SettingsService : ISettingsService {
        public static SettingsService Instance { get; }
        static SettingsService() {
            // implement singleton pattern
            Instance = Instance ?? new SettingsService();
        }

        readonly Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService() {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool UseShellBackButton {
            get { return _helper.Read(nameof(UseShellBackButton), true); }
            set {
                _helper.Write(nameof(UseShellBackButton), value);
                ApplyUseShellBackButton(value);
            }
        }

        public bool ScrollToPostWithImageAfterViewingImage {
            get {
                var value = _helper.Read(nameof(ScrollToPostWithImageAfterViewingImage), false);
                return value;
            }

            set {
                _helper.Write(nameof(ScrollToPostWithImageAfterViewingImage), value);
            }
        }

        public Theme AppTheme {
            get {
                var theme = Theme.System;
                var value = _helper.Read(nameof(AppTheme), theme.ToString());
                return Enum.TryParse(value, out theme) ? theme : Theme.System;
            }
            set {
                _helper.Write(nameof(AppTheme), value.ToString());
                ApplyAppTheme(value);
            }
        }

        public RepliesViewMode RepliesViewMode {
            get { return _helper.Read(nameof(RepliesViewMode), RepliesViewMode.Auto); }
            set {
                _helper.Write(nameof(RepliesViewMode), value);
            }
        }

        public TimeSpan CacheMaxDuration {
            get { return _helper.Read(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set {
                _helper.Write(nameof(CacheMaxDuration), value);
                ApplyCacheMaxDuration(value);
            }
        }
    }
}

