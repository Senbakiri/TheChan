using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Template10.Common;
using Template10.Services.NavigationService;
using Win2ch.Models;
using Win2ch.Views;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagesViewPage : Page
    {
        private int _currentIndex;

        public ImagesViewPage()
        {
            ImagesSources = new ObservableCollection<BitmapImage>();
            this.InitializeComponent();
            ImagesList.ItemsSource = ImagesSources;
        }

        private double _totalManupulationYOffset;

        public ObservableCollection<BitmapImage> ImagesSources  { get; }

        private List<ImageInfo>  _AllImages;

        public List<ImageInfo> AllImages
        {
            get { return _AllImages; }
            set
            {
                _AllImages = value;

                ImagesSources.Clear();
                foreach (var imageInfo in AllImages)
                {
                    ImagesSources.Add(new BitmapImage(new Uri(imageInfo.Url, UriKind.Absolute)));
                }
            }
        }
        
        

        private int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (value >= ImagesSources.Count)
                    _currentIndex = 0;
                else if (value < 0)
                    _currentIndex = ImagesSources.Count - 1;
                else
                    _currentIndex = value;

                if (_currentIndex < 0)
                    return;

                ImagesList.SelectedIndex = CurrentIndex;
                //ScrollViewer.ChangeView(0, 0, 1.0f, true);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var data = e.Parameter as Tuple<ImageInfo, List<ImageInfo>>;
            if (data == null)
                throw new ArgumentException();

            var isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";
            if (isMobile)
                await StatusBar.GetForCurrentView().HideAsync();

            AllImages = data.Item2;
            CurrentIndex = AllImages.IndexOf(data.Item1);

            Shell.HamburgerMenu.IsFullScreen = true;
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";
            if (isMobile)
                await StatusBar.GetForCurrentView().ShowAsync();

            Shell.HamburgerMenu.IsFullScreen = false;

            base.OnNavigatingFrom(e);
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Escape && e.Key != VirtualKey.Back) return;

            e.Handled = true;
            Close();
        }

        private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Close();
        }

        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            if (parentElement == null) return null;
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                var item = child as T;
                if (item != null)
                    return item;

                var result = FindFirstElementInVisualTree<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            for (var i = 0; i < AllImages.Count; i++)
            {
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
        
        public void Close()
        {
            var nav = BootStrapper.Current.NavigationService;
            nav.GoBack();
        }

        private void ImagesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var view = sender as FlipView;

            if (view == null) return;

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
    }

    public class ImageViewCloseEventArgs
    { }
}
