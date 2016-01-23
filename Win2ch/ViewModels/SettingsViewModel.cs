using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Template10.Mvvm;
using Win2ch.Services.SettingsServices;
using ViewModelBase = Win2ch.Mvvm.ViewModelBase;

namespace Win2ch.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private ISettingsService _settingsService;

        public string Version { get; }
        public string ReleaseNotes { get; private set; }
        public List<Theme> AvailableThemes { get; } = Enum.GetValues(typeof (Theme)).Cast<Theme>().ToList();
        public List<RepliesViewMode> AvailableRepliesViewModes
        { get; } = Enum.GetValues(typeof(RepliesViewMode)).Cast<RepliesViewMode>().ToList();

        public Theme SelectedTheme
        {
            get { return _settingsService.AppTheme; }
            set
            {
                _settingsService.AppTheme = value;
                RaisePropertyChanged();
            }
        }

        public RepliesViewMode SelectedRepliesViewMode
        {
            get { return _settingsService.RepliesViewMode; }
            set
            {
                _settingsService.RepliesViewMode = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            _settingsService = SettingsService.Instance;

            var id = Package.Current.Id;
            Version = $"v{id.Version.Major}.{id.Version.Minor}.{id.Version.Build}";
            LoadReleaseNotes();
        }

        private async void LoadReleaseNotes()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ReleaseNotes.txt"));
            var stream = await file.OpenReadAsync();
            var reader = new StreamReader(stream.AsStreamForRead());
            ReleaseNotes = reader.ReadToEnd();
        }
    }
}
