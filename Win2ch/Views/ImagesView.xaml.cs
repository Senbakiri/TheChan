using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Core.Models;
using FFImageLoading.Args;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ImagesView {
        public ImagesView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ImagesViewModel;
        }

        public ImagesViewModel ViewModel { get; private set; }

        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject {
            if (parentElement == null) return null;
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++) {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                var item = child as T;
                if (item != null)
                    return item;

                var result = FindFirstElementInVisualTree<T>(child);
                if (result != null) {
                    return result;
                }
            }

            return null;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            for (var i = 0; i < ViewModel.Attachments.Count; i++) {
                var flipViewItem = this.FlipView.ContainerFromIndex(i);
                var scrollViewItem = FindFirstElementInVisualTree<ScrollViewer>(flipViewItem);
                var imageItem = FindFirstElementInVisualTree<Image>(scrollViewItem);
                if (scrollViewItem == null || imageItem == null) continue;

                scrollViewItem.Height = e.NewSize.Height;
                scrollViewItem.Width = e.NewSize.Width;
                imageItem.MaxHeight = e.NewSize.Height;
                imageItem.MaxWidth = e.NewSize.Width;
                scrollViewItem.ChangeView(0, 0, 1.0f, true);
            }
        }

        private void FlipView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            var view = sender as FlipView;

            if (view == null || ViewModel.CurrentAttachment == null) return;

            DependencyObject flipViewItem = view.ContainerFromIndex(view.SelectedIndex);
            var scrollViewItem = FindFirstElementInVisualTree<ScrollViewer>(flipViewItem);
            var imageItem = FindFirstElementInVisualTree<Image>(scrollViewItem);

            if (scrollViewItem == null || imageItem == null) return;

            scrollViewItem.Height = ActualHeight;
            scrollViewItem.Width = ActualWidth;
            imageItem.MaxHeight = ActualHeight;
            imageItem.MaxWidth = ActualWidth;
            scrollViewItem.ChangeView(0, 0, 1.0f, true);

        }

        private void ScrollViewer_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
            var scrollViewer = (ScrollViewer)sender;
            var item = (FrameworkElement)scrollViewer.Content;
            if (item == null)
                return;

            if (scrollViewer.ZoomFactor > 1) {
                item.ManipulationMode = ManipulationModes.System;
            } else {
                item.ManipulationMode = ManipulationModes.System |
                                        ManipulationModes.TranslateY |
                                        ManipulationModes.TranslateInertia;
            }
        }

        private void Image_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            var elem = (FrameworkElement)sender;
            double total = e.Cumulative.Translation.Y;
            var translate = elem.RenderTransform as TranslateTransform;
            if (translate == null)
                elem.RenderTransform = translate = new TranslateTransform();

            if (e.IsInertial && Math.Abs(total) > 500) {
                e.Complete();
                return;
            }

            translate.Y = e.Cumulative.Translation.Y;

            this.Underlay.Opacity = 1 - Math.Abs(total) / 300;
        }

        private void Image_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            var elem = (FrameworkElement)sender;

            var transform = elem.RenderTransform as TranslateTransform;
            if (transform != null) {
                transform.X = 0;
                transform.Y = 0;
            }

            elem.Opacity = this.Underlay.Opacity = 1;
            if (Math.Abs(e.Cumulative.Translation.Y) > 150)
                ViewModel.RequestClosing();
        }


        private void Image_OnError(object sender, ErrorEventArgs e) {
            // TODO: Implement error message
        }
    }

}
