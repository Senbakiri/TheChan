using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Win2ch.Models;
using Win2ch.Views;

namespace Win2ch.Controls {
    public sealed partial class ImagesViewer {

        public ImagesViewer() {
            InitializeComponent();
        }

        public bool IsOpened { get; private set; }

        public ObservableCollection<BitmapImage> ImagesSources { get; }
        = new ObservableCollection<BitmapImage>();

        public event EventHandler<ImagesViewerCloseEventArgs> OnClose = delegate { };

        private List<ImageInfo> _AllImages;

        public List<ImageInfo> AllImages {
            get { return _AllImages; }
            set {
                _AllImages = value;

                CurrentImage = null;
                ImagesSources.Clear();
                if (_AllImages == null)
                    return;

                foreach (var imageInfo in AllImages) {
                    ImagesSources.Add(new BitmapImage(new Uri(imageInfo.Url, UriKind.Absolute)));
                }
            }
        }
        
        public BitmapImage CurrentImage {
            get { return ImagesList.SelectedItem as BitmapImage; }
            set {
                ImagesList.SelectedItem = value;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            Shell.HamburgerMenu.IsFullScreen = false;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key != VirtualKey.Escape && e.Key != VirtualKey.Back) return;

            e.Handled = true;
            Close();
        }

        private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e) {
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
        }


        public void Show(ImageInfo currentImage, List<ImageInfo> allImages) {
            IsOpened = true;
            Visibility = Visibility.Visible;
            AllImages = allImages;
            CurrentImage = ImagesSources.FirstOrDefault(im => im.UriSource.OriginalString.Equals(currentImage.Url));

            Shell.HamburgerMenu.IsFullScreen = true;
        }

        public void Close() {
            Visibility = Visibility.Collapsed;
            Shell.HamburgerMenu.IsFullScreen = false;
            IsOpened = false;
            var lastImage = AllImages.FirstOrDefault(i => i.Url == CurrentImage.UriSource.OriginalString);
            OnClose(this, new ImagesViewerCloseEventArgs(lastImage));
        }
    }

    public class ImagesViewerCloseEventArgs {
        public ImageInfo LastImage { get; }

        public ImagesViewerCloseEventArgs(ImageInfo lastImage) {
            LastImage = lastImage;
        }
    }
}
