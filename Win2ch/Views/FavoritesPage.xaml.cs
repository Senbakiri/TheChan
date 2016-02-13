using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Win2ch.Models;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class FavoritesPage {
        public FavoritesViewModel ViewModel { get; private set; }

        private Dictionary<StoredThreadInfo, ScaleEffect> _scaleEffect = new Dictionary<StoredThreadInfo, ScaleEffect>();

        private Dictionary<StoredThreadInfo, GaussianBlurEffect> _blurEffect =
            new Dictionary<StoredThreadInfo, GaussianBlurEffect>();
        private Dictionary<StoredThreadInfo, bool> _isImageLoaded = new Dictionary<StoredThreadInfo, bool>();
        private Dictionary<StoredThreadInfo, CanvasBitmap> _image = new Dictionary<StoredThreadInfo, CanvasBitmap>();

        public FavoritesPage() {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            ViewModel = (FavoritesViewModel) DataContext;
        }

        private void FavoriteThreads_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.GoToThread((StoredThreadInfo) e.ClickedItem);
        }

        private void ResizeThreads() {
            if ((int)ThreadsPivotItem.ActualWidth == 0 || ThreadsPivotItem.ActualWidth > 500)
                return;

            if (FavoriteThreadsGrid.ItemsPanelRoot == null)
                return;

            foreach (var item in FavoriteThreadsGrid.ItemsPanelRoot.Children.Cast<GridViewItem>()) {
                var child = item.ContentTemplateRoot as FrameworkElement;
                if (child != null)
                    child.Width = Math.Floor(ThreadsPivotItem.ActualWidth) / 2 - (item.Margin.Left + item.Margin.Right)
                        - (child.Margin.Left + child.Margin.Right) - 1;
            }
        }

        private async void RemoveThreadFromFavoritesMenuFlyoutItem_OnClick(object sender, RoutedEventArgs e) {
            await ViewModel.RemoveThreadFromFavorites((StoredThreadInfo)((FrameworkElement) sender).DataContext);
        }
    }
}
