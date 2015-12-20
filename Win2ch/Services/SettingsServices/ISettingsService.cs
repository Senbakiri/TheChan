using System;
using Windows.UI.Xaml;
using Win2ch.Attributes;

namespace Win2ch.Services.SettingsServices
{

    public interface ISettingsService
    {
        bool UseShellBackButton { get; set; }
        Theme AppTheme { get; set; }
        TimeSpan CacheMaxDuration { get; set; }
    }
}
