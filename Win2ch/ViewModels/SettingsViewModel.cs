using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Win2ch.Mvvm;
using Win2ch.Services.SettingsServices;

namespace Win2ch.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        private ISettingsService _settingsService;

        public string Version { get; }
        public List<Theme> AvailableThemes { get; } = Enum.GetValues(typeof (Theme)).Cast<Theme>().ToList();

        public Theme SelectedTheme
        {
            get { return _settingsService.AppTheme; }
            set
            {
                _settingsService.AppTheme = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            _settingsService = SettingsService.Instance;

            var id = Package.Current.Id;
            Version = $"{id.Version.Major}.{id.Version.Minor}.{id.Version.Build}.{id.Version.Revision}";
        }
    }
}
