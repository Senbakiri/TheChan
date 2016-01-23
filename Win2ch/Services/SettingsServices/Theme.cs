using Win2ch.Attributes;

namespace Win2ch.Services.SettingsServices
{
    public enum Theme
    {
        [Display("Светлая")]
        Light,

        [Display("Темная")]
        Dark,

        [Display("Из настроек системы")]
        System,
    }
}