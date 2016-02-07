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
using Win2ch.Services;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class FavoritesPage {
        public FavoritesViewModel ViewModel { get; private set; }

        private Dictionary<FavoriteThread, ScaleEffect> _scaleEffect = new Dictionary<FavoriteThread, ScaleEffect>();

        private Dictionary<FavoriteThread, GaussianBlurEffect> _blurEffect =
            new Dictionary<FavoriteThread, GaussianBlurEffect>();
        private Dictionary<FavoriteThread, bool> _isImageLoaded = new Dictionary<FavoriteThread, bool>();
        private Dictionary<FavoriteThread, CanvasBitmap> _image = new Dictionary<FavoriteThread, CanvasBitmap>();

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
                    child.Width = Math.Floor(ThreadsPivotItem.ActualWidth) / 2 - (item.Margin.Left + item.Margin.Right)
                        - (child.Margin.Left + child.Margin.Right) - 1;
            }
        }

        private void CanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args) {
            var thread = (FavoriteThread) sender.DataContext;
            if (!_isImageLoaded[thread])
                return;

            using (var session = args.DrawingSession) {
                var image = _image[thread];
                var scaleEffect = _scaleEffect[thread];
                var blurEffect = _blurEffect[thread];
                session.Units = CanvasUnits.Pixels;

                var displayScaling = DisplayInformation.GetForCurrentView().LogicalDpi / 96.0;

                var pixelWidth = sender.ActualWidth * displayScaling;

                var scalefactor = pixelWidth / image.Size.Width;

                if ((int) scalefactor == 0)
                    return;

                scaleEffect.Source = image;
                scaleEffect.Scale = new Vector2 {
                    X = (float)scalefactor,
                    Y = (float)scalefactor
                };

                blurEffect.Source = scaleEffect;
                blurEffect.BlurAmount = 10;

                session.DrawImage(blurEffect, 0.0f, 0.0f);
            }
        }

        private async void CanvasControl_OnCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args) {
            var thread = (FavoriteThread)sender.DataContext;
            _isImageLoaded[thread] = false;
            if (!string.IsNullOrWhiteSpace(thread.ThumbnailUrl)) {
                _scaleEffect[thread] = new ScaleEffect();
                _blurEffect[thread] = new GaussianBlurEffect();
                _image[thread] = await CanvasBitmap.LoadAsync(sender.Device,
                    new Uri(thread.ThumbnailUrl));
                _isImageLoaded[thread] = true;
                sender.Invalidate();
            }
        }

        private async void RemoveThreadFromFavoritesMenuFlyoutItem_OnClick(object sender, RoutedEventArgs e) {
            await ViewModel.RemoveThreadFromFavorites((FavoriteThread)((FrameworkElement) sender).DataContext);
        }
    }
}
