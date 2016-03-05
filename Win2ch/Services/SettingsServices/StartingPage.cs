using Win2ch.Attributes;

namespace Win2ch.Services.SettingsServices {
    public enum StartingPage {
        [Display("Главная")]
        Main,

        [Display("Избранное")]
        Favorites,

        [Display("Недавние треды")]
        RecentThreads
    }
}