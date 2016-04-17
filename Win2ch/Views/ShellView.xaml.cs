using System.ComponentModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Win2ch.Common;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ShellView {
        public ShellView() {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            SetupTitleBar();
            if (DeviceUtils.GetDeviceFamily() == DeviceFamily.Mobile) {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
        }

        private void OnDataContextChanged(FrameworkElement s, DataContextChangedEventArgs e) {
            if (ViewModel != null)
                ViewModel.LoadingInfo.PropertyChanged -= LoadingInfoOnPropertyChanged;
            ViewModel = DataContext as ShellViewModel;
            if (ViewModel == null)
                return;
            ViewModel.LoadingInfo.PropertyChanged += LoadingInfoOnPropertyChanged;
        }

        private void LoadingInfoOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ViewModel.LoadingInfo.State))
                SetVisualStateForLoadingState();
        }

        private void SetVisualStateForLoadingState() {
            string commonStateName, concreteStateName;
            switch (ViewModel.LoadingInfo.State) {
                case LoadingState.InProgress:
                    commonStateName = "Loading";
                    concreteStateName = "Progress";
                    break;
                case LoadingState.Success:
                    commonStateName = "Idle";
                    concreteStateName = "Success";
                    break;
                case LoadingState.Error:
                    commonStateName = ViewModel.LoadingInfo.IsTryingAgainEnabled ? "Loading" : "Idle";
                    concreteStateName = "Error";
                    break;
                default:
                    commonStateName = "Idle";
                    concreteStateName = "Progress";
                    break;
            }

            VisualStateManager.GoToState(this, commonStateName, true);
            VisualStateManager.GoToState(this, concreteStateName, true);
        }

        private ShellViewModel ViewModel { get; set; }

        private void SetupTitleBar() {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            var bg = (SolidColorBrush) Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            titleBar.BackgroundColor =
                titleBar.ButtonBackgroundColor =
                    titleBar.ButtonInactiveBackgroundColor =
                        titleBar.InactiveBackgroundColor = bg.Color;
        }

        private void LoadingInfoPanel_OnTapped(object sender, TappedRoutedEventArgs e) {
            VisualStateManager.GoToState(this, "Idle", false);
        }
    }
}
