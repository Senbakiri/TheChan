using Win2ch.Attributes;

namespace Win2ch.Services.SettingsServices {
    public enum RepliesViewMode {
        [Display("Авто")]
        Auto,

        [Display("Дерево")]
        Tree,

        [Display("Лента")]
        List,
    }
}