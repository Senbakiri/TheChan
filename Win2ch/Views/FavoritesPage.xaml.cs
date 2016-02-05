using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Win2ch.Models;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class FavoritesPage {
        public FavoritesViewModel ViewModel { get; private set; }

        public FavoritesPage() {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            ViewModel = (FavoritesViewModel) DataContext;
        }

        private void FavoriteThreads_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.GoToThread((FavoriteThread) e.ClickedItem);
        }

        private void ResizeThreads() {
            if ((int)ThreadsPivotItem.ActualWidth == 0 || ThreadsPivotItem.ActualWidth > 500)
                return;

            if (FavoriteThreadsGrid.ItemsPanelRoot == null)
                return;

            foreach (var item in FavoriteThreadsGrid.ItemsPanelRoot.Children.Cast<GridViewItem>()) {
                var child = item.ContentTemplateRoot as FrameworkElement;
                if (child != null)
                    child.Width = ThreadsPivotItem.ActualWidth / 2 - (item.Margin.Left + item.Margin.Right)
                        - (child.Margin.Left + child.Margin.Right);
            }
        }
    }
}
