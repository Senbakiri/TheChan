using Windows.System;
using Windows.UI.Xaml;
using Win2ch.Models;
using Win2ch.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Win2ch.Views {
    public sealed partial class MainPage {
        public MainPage() {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.NavigateToBoard(e.ClickedItem as Board);
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e) {
            ViewModel.LoadBoards();
        }

        private void FastNavigation_OnKeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key != VirtualKey.Enter)
                return;

            e.Handled = true;
            ViewModel.NavigateToBoard(new Board(FastNavigation.Text.ToLowerInvariant()));
        }
    }
}
