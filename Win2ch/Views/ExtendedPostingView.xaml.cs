using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Template10.Utils;
using Win2ch.Common.UI;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ExtendedPostingView {
        public ExtendedPostingView(IShell shell) {
            Shell = shell;
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ExtendedPostingViewModel;
        }

        private IShell Shell { get; }
        public ExtendedPostingViewModel ViewModel { get; set; }

        private void ExtendedPostingView_OnLoaded(object sender, RoutedEventArgs e) {
            if (DeviceUtils.CurrentDeviceFamily != DeviceUtils.DeviceFamilies.Mobile) {
                this.Window.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        private void Close() {
            Shell.HidePopup();
        }
    }
}
