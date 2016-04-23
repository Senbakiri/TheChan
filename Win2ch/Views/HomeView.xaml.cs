using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Core.Models;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class HomeView {
        public HomeView(SettingsViewModel settingsViewModel, FavoritesViewModel favoritesViewModel) {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as HomeViewModel;
            SettingsViewModel = settingsViewModel;
            FavoritesViewModel = favoritesViewModel;
        }

        private HomeViewModel ViewModel { get; set; }
        private SettingsViewModel SettingsViewModel { get; }
        private FavoritesViewModel FavoritesViewModel { get; }

        private void BoardsListView_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.NavigateToBoard(e.ClickedItem as BriefBoardInfo);
        }

        private void FastNavigationTextBox_OnTextChanged(object sender, TextChangedEventArgs e) {
            this.CategoriesCvs.Source = ViewModel.FilterItems(this.FastNavigation.Text);
        }

        private void FastNavigationTextBox_OnKeyUp(object sender, KeyRoutedEventArgs e) {
            if (e.Key == VirtualKey.Enter)
                ViewModel.NavigateByString(this.FastNavigation.Text);
        }

        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            switch (this.Pivot.SelectedIndex) {
                case 1:
                    FavoritesViewModel.Load();
                    break;
            }
        }
    }
}
