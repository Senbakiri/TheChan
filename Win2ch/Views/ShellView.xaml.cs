using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ShellView {
        public ShellView() {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            SetupTitleBar();
        }

        private void OnDataContextChanged(FrameworkElement s, DataContextChangedEventArgs e) {
            ViewModel = DataContext as ShellViewModel;
        }

        private ShellViewModel ViewModel { get; set; }

        private void SetupTitleBar() {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            var bg = (SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            titleBar.BackgroundColor =
                titleBar.ButtonBackgroundColor =
                titleBar.ButtonInactiveBackgroundColor =
                titleBar.InactiveBackgroundColor = bg.Color;
        }
    }
}
