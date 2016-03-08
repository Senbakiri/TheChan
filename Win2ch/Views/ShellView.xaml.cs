using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ShellView {
        public ShellView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ShellViewModel;
            SetupTitleBar();
        }

        private ShellViewModel ViewModel { get; set; }

        private void SetupTitleBar() {
            CoreApplicationViewTitleBar coreBar = CoreApplication.GetCurrentView().TitleBar;
            if (coreBar == null)
                return;

            coreBar.ExtendViewIntoTitleBar = true;
            coreBar.LayoutMetricsChanged += TitleBarOnLayoutMetricsChanged;
            Window.Current.SetTitleBar(Bar);

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = (Color)Application.Current.Resources["SystemAltHighColor"];
        }

        private void TitleBarOnLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args) {
            Bar.Height = sender.Height;
            Bar.MinWidth = sender.SystemOverlayRightInset;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            TabsList.MaxWidth = ActualWidth - Bar.MinWidth;
        }
    }
}
