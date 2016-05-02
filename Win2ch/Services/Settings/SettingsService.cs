using Template10.Services.SettingsService;

namespace Win2ch.Services.Settings {
    public class SettingsService : ISettingsService {
        private SettingsHelper Helper { get; } = new SettingsHelper();

        public double FontScale {
            get { return Helper.Read(nameof(FontScale), 1.0); }
            set { Helper.Write(nameof(FontScale), value); }
        }
    }
}