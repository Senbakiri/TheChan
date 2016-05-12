namespace TheChan.Services.Settings {
    public interface ISettingsService {
        double FontScale { get; set; } 
        Theme CurrentTheme { get; set; }
    }
}