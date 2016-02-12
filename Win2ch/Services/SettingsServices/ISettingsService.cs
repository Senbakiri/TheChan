using System;
using Windows.UI.Xaml;
using Win2ch.Attributes;

namespace Win2ch.Services.SettingsServices {

    public interface ISettingsService {
        bool UseShellBackButton { get; set; }
        bool ScrollToPostWithImageAfterViewingImage { get; set; }
        int MaxLinesInPostOnBoard { get; set; }
        Theme AppTheme { get; set; }
        RepliesViewMode RepliesViewMode { get; set; }
        TimeSpan CacheMaxDuration { get; set; }
    }
}
