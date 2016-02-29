using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Web.Http;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Template10.Controls;
using Win2ch.Annotations;
using Win2ch.Common;
using Win2ch.Models;
using XamlAnimatedGif;

namespace Win2ch.Controls {
    public class ImageWrapper : INotifyPropertyChanged {
        private int _LoadingProgress;
        private bool _IsLoading;
        private string _LoadingString;

        public Attachment Image { get; }

        public int LoadingProgress {
            get { return _LoadingProgress; }
            private set {
                if (value == _LoadingProgress)
                    return;
                _LoadingProgress = value;
                LoadingString = FormatLoadingString();
                IsLoading = LoadingProgress < 100;
                RaisePropertyChanged();
            }
        }

        public bool IsLoading {
            get { return _IsLoading; }
            private set {
                if (value == _IsLoading)
                    return;
                _IsLoading = value;
                RaisePropertyChanged();
            }
        }

        public string LoadingString {
            get { return _LoadingString; }
            private set {
                if (value == _LoadingString)
                    return;
                _LoadingString = value;
                RaisePropertyChanged();
            }
        }

        static ImageWrapper() {
            AnimationBehavior.DownloadProgress += BitmapImageOnDownloadProgress;
            AnimationBehavior.Error += BitmapImageOnImageFailed;
        }

        public ImageWrapper(Attachment image) {
            Image = image;
            LoadingString = FormatLoadingString();
        }

        private static void BitmapImageOnImageFailed(object sender, AnimationErrorEventArgs e) {
            var wrapper = sender.GetDataContext<ImageWrapper>();
            wrapper.IsLoading = true;
            wrapper.LoadingString = "Произошла ошибка" + " (" + e.Exception + ")";
        }

        private static void BitmapImageOnDownloadProgress(object sender, DownloadProgressEventArgs e) {
            var wrapper = sender.GetDataContext<ImageWrapper>();
            wrapper.LoadingProgress = e.Progress;
        }

        private string FormatLoadingString() {
            var downloaded = (int)((double) Image.Size/100*LoadingProgress);
            return $"{downloaded:D} / {Image.Size} KB";
        }

        public override bool Equals(object obj) {
            return obj is ImageWrapper && Equals((ImageWrapper) obj);
        }

        protected bool Equals(ImageWrapper other) {
            return Equals(Image, other.Image);
        }

        public override int GetHashCode() {
            return Image?.GetHashCode() ?? 0;
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public sealed partial class ImagesViewer : INotifyPropertyChanged {


        public ObservableItemCollection<ImageWrapper> Images { get; }

        public event EventHandler<ImagesViewerClosedEventArgs> Closed = delegate { };

        private List<Attachment> _AllImages;
        private ImageWrapper _CurrentImage;
        private int _CurrentIndex = -1;
        private bool _IsInfoPanelVisible = true;

        public List<Attachment> AllImages {
            get { return _AllImages; }
            set {
                _AllImages = value;

                Images.Clear();
                if (_AllImages == null)
                    return;

                foreach (var imageInfo in AllImages) {
                    Images.Add(new ImageWrapper(imageInfo));
                }
            }
        }

        public ImageWrapper CurrentImage {
            get { return _CurrentImage; }
            set {
                if (Equals(value, _CurrentImage))
                    return;
                _CurrentImage = value;
                CurrentIndex = Images.IndexOf(CurrentImage);
                RaisePropertyChanged();
            }
        }

        public int CurrentIndex {
            get { return _CurrentIndex; }
            set {
                if (value == _CurrentIndex)
                    return;
                _CurrentIndex = value;
                if (CurrentIndex >= 0 && CurrentIndex < Images.Count) {
                    CurrentImage = Images[CurrentIndex];
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

        public ImagesViewer(Attachment currentImage, List<Attachment> allImages) {
            Images = new ObservableItemCollection<ImageWrapper>();
            InitializeComponent();
            AllImages = allImages;
            ImagesList.ItemsSource = Images;
            CurrentImage = Images.FirstOrDefault(im => im.Image.Url.Equals(currentImage.Url));
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
            var lastImage = CurrentImage.Image;
            Closed(this, new ImagesViewerClosedEventArgs(lastImage));
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
        
        private void Root_OnTapped(object sender, TappedRoutedEventArgs e) {
            IsInfoPanelVisible = !IsInfoPanelVisible;
        }

        private async void SaveImageAsMenuFlyoutItem_OnClick(object sender, RoutedEventArgs e) {
            var picker = new FileSavePicker {
                SuggestedFileName = CurrentImage.Image.Name.Split('.')[0]
            };
            picker.FileTypeChoices.Add("JPEG file", new[] { ".jpg" });

            try {
                var file = await picker.PickSaveFileAsync();
                var client = new HttpClient();
                var resp = await client.GetAsync(new Uri(CurrentImage.Image.Url));
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
            await Launcher.LaunchUriAsync(new Uri(CurrentImage.Image.Url));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ImagesViewerClosedEventArgs {
        public Attachment LastImage { get; }

        public ImagesViewerClosedEventArgs(Attachment lastImage) {
            LastImage = lastImage;
        }
    }
}
