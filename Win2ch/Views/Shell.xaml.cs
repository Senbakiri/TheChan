using System.ComponentModel;
using Windows.System.Profile;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Win2ch.Views {
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplitView
    public sealed partial class Shell : INotifyPropertyChanged {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        public Shell(INavigationService navigationService) {
            Instance = this;
            InitializeComponent();
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(Application.Current.RequestedTheme);
            SetupColors();
        }

        public bool IsBusy { get; set; }
        public string BusyText { get; set; } = "Please wait...";
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetupColors() {
            var bg = (Color)BootStrapper.Current.Resources["SystemChromeMediumColor"];
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor =
                titleBar.ButtonBackgroundColor =
                titleBar.ButtonInactiveBackgroundColor =
                titleBar.InactiveBackgroundColor = bg;

            Window.Current.CoreWindow.Activated += (s, e) => SetupColors();

        }

        public static void SetBusy(bool busy, string text = null) {
            WindowWrapper.Current().Dispatcher.Dispatch(() => {
                if (busy)
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                else
                    BootStrapper.Current.UpdateShellBackButton();

                Instance.IsBusy = busy;
                Instance.BusyText = text;

                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(IsBusy)));
                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(BusyText)));
            });
        }
    }
}

