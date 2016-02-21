using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.Web.Http;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Win2ch.Annotations;
using Win2ch.Common;
using Win2ch.Models;

namespace Win2ch.Controls {
    public sealed partial class ImagesViewer :  INotifyPropertyChanged {
        public ObservableCollection<BitmapImage> ImagesSources { get; }

        public event EventHandler<ImagesViewerCloseEventArgs> OnClose = delegate { };

        private List<ImageInfo> _AllImages;
        private BitmapImage _CurrentImage;
        private int _CurrentIndex = -1;
        private ImageInfo _CurrentImageInfo;
        private bool _IsInfoPanelVisible = true;

        public List<ImageInfo> AllImages {
            get { return _AllImages; }
            set {
                _AllImages = value;

                ImagesSources.Clear();
                if (_AllImages == null)
                    return;

                foreach (var imageInfo in AllImages) {
                    ImagesSources.Add(new BitmapImage(new Uri(imageInfo.Url, UriKind.Absolute)));
                }
            }
        }

        public BitmapImage CurrentImage {
            get { return _CurrentImage; }
            set {
                if (Equals(value, _CurrentImage))
                    return;
                _CurrentImage = value;
                CurrentIndex = ImagesSources.IndexOf(value);
            }
        }

        public ImageInfo CurrentImageInfo {
            get { return _CurrentImageInfo; }
            set {
                if (Equals(value, _CurrentImageInfo))
                    return;
                _CurrentImageInfo = value;
                RaisePropertyChanged();
            }
        }

        public int CurrentIndex {
            get { return _CurrentIndex; }
            set {
                if (value == _CurrentIndex)
                    return;
                _CurrentIndex = value;
                if (CurrentIndex >= 0 && CurrentIndex < ImagesSources.Count) {
                    _CurrentImage = ImagesSources[CurrentIndex];
                    CurrentImageInfo = AllImages[CurrentIndex];
                    ImagesList.SelectedIndex = value;
                }
            }
        }

        public bool IsInfoPanelVisible {
            get { return _IsInfoPanelVisible; }
            set {
                _IsInfoPanelVisible = value;
                VisualStateManager.GoToState(this, value ? "Visible" : "Hidden", true);
            }
        }

        public ImagesViewer(ImageInfo currentImage, List<ImageInfo> allImages) {
            ImagesSources = new ObservableCollection<BitmapImage>();
            InitializeComponent();
            ImagesList.ItemsSource = ImagesSources;
            AllImages = allImages;
            CurrentImage = ImagesSources.FirstOrDefault(im => im.UriSource.OriginalString.Equals(currentImage.Url));
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key != VirtualKey.Escape && e.Key != VirtualKey.Back) return;

            e.Handled = true;
            Close();
        }

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
            if (AllImages == null)
                return;

            for (var i = 0; i < AllImages.Count; i++) {
                var flipViewItem = ImagesList.ContainerFromIndex(i);
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

        private void ImagesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            var view = sender as FlipView;

            if (view == null || CurrentImage == null) return;

            var flipViewItem = view.ContainerFromIndex(view.SelectedIndex);
            var scrollViewItem = FindFirstElementInVisualTree<ScrollViewer>(flipViewItem);
            var imageItem = FindFirstElementInVisualTree<Image>(scrollViewItem);

            if (scrollViewItem == null || imageItem == null) return;

            scrollViewItem.Height = ActualHeight;
            scrollViewItem.Width = ActualWidth;
            imageItem.MaxHeight = ActualHeight;
            imageItem.MaxWidth = ActualWidth;
            scrollViewItem.ChangeView(0, 0, 1.0f, true);

            if (CurrentIndex != ImagesList.SelectedIndex)
                CurrentIndex = ImagesList.SelectedIndex;
        }

        public void Close() {
            var lastImage = AllImages.FirstOrDefault(i => i.Url == CurrentImage.UriSource.OriginalString);
            OnClose(this, new ImagesViewerCloseEventArgs(lastImage));
        }

        private void Underlay_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            IsInfoPanelVisible = false;
            var elem = (FrameworkElement)sender;
            var scrollViewer = (ScrollViewer) elem.Parent;
            var total = e.Cumulative.Translation.Y;
            var translate = elem.RenderTransform as TranslateTransform;
            if (translate == null)
                elem.RenderTransform = translate = new TranslateTransform();

            if (scrollViewer.ZoomFactor > 1) {
                    scrollViewer.ChangeView(null,
                        scrollViewer.VerticalOffset - e.Delta.Translation.Y,
                        null, false);
            } else {
                if (e.IsInertial && Math.Abs(total) > 500) {
                    e.Complete();
                    return;
                }

                translate.Y = e.Cumulative.Translation.Y;

                Underlay.Opacity = 1 - Math.Abs(total) / 300;
            }

            
        }

        private void Underlay_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            var elem = (FrameworkElement)sender;

            if (((ScrollViewer) elem.Parent).ZoomFactor > 1)
                return;

            var transform = elem.RenderTransform as TranslateTransform;
            if (transform != null) {
                transform.X = 0;
                transform.Y = 0;
            }
            elem.Opacity = Underlay.Opacity = 1;
            if (Math.Abs(e.Cumulative.Translation.Y) > 150)
                Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Root_OnTapped(object sender, TappedRoutedEventArgs e) {
            IsInfoPanelVisible = !IsInfoPanelVisible;
        }

        private async void SaveImageAsMenuFlyoutItem_OnClick(object sender, RoutedEventArgs e) {
            var picker = new FileSavePicker {
                SuggestedFileName = CurrentImageInfo.Name.Split('.')[0]
            };
            picker.FileTypeChoices.Add("JPEG file", new[] { ".jpg" });

            try {
                var file = await picker.PickSaveFileAsync();
                var client = new HttpClient();
                var resp = await client.GetAsync(new Uri(CurrentImageInfo.Url));
                var opened = await file.OpenAsync(FileAccessMode.ReadWrite);
                await resp.Content.WriteToStreamAsync(opened);
                opened.Dispose();
                await new MessageDialog("Изображение успешно сохранено.").ShowAsync();
            } catch (Exception ex) {
                await Utils.ShowOtherError(ex, "Не удалось сохранить изображение");
            }
        }

        private void Image_OnHolding(object sender, HoldingRoutedEventArgs e) {
            var senderFrameworkElem = (FrameworkElement)sender;
            var flyout = FlyoutBase.GetAttachedFlyout(senderFrameworkElem) as MenuFlyout;
            flyout?.ShowAt(senderFrameworkElem, e.GetPosition(senderFrameworkElem));
        }

        private void Image_OnRightTapped(object sender, RightTappedRoutedEventArgs e) {
            var senderFrameworkElem = (FrameworkElement)sender;
            var flyout = FlyoutBase.GetAttachedFlyout(senderFrameworkElem) as MenuFlyout;
            flyout?.ShowAt(senderFrameworkElem, e.GetPosition(senderFrameworkElem));
        }

        private async void OpenInBrowser_OnClick(object sender, RoutedEventArgs e) {
            await Launcher.LaunchUriAsync(new Uri(CurrentImageInfo.Url));
        }
    }

    public class ImagesViewerCloseEventArgs {
        public ImageInfo LastImage { get; }

        public ImagesViewerCloseEventArgs(ImageInfo lastImage) {
            LastImage = lastImage;
        }
    }
}
