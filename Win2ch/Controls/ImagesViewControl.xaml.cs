using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Win2ch.Models;
using Win2ch.Views;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagesViewControl : Page
    {
        private int _currentIndex;

        public delegate void ImageViewCloseHandler(object sender, ImageViewCloseEventArgs e);

        public ImagesViewControl()
        {
            this.InitializeComponent();
        }
        
        private List<ImageInfo> AllImages { get; set; }

        public event ImageViewCloseHandler Close = delegate { };
        

        private int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (value >= AllImages.Count)
                    _currentIndex = 0;
                else if (value < 0)
                    _currentIndex = AllImages.Count - 1;
                else
                    _currentIndex = value;

                if (_currentIndex < 0)
                    return;

                CurrentImage.Source = new BitmapImage(new Uri(AllImages[CurrentIndex].Url, UriKind.Absolute));
                ScrollViewer.ChangeView(0, 0, 1.0f, true);
            }
        }

        public async void Show(ImageInfo imageInfo, IEnumerable<ImageInfo> allImages)
        {
            var isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";
            if (isMobile)
                await StatusBar.GetForCurrentView().HideAsync();

            Shell.HamburgerMenu.IsFullScreen = true;
            AllImages = allImages.ToList();
            CurrentIndex = AllImages.IndexOf(imageInfo);
        }

        private void Background_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Close(this, new ImageViewCloseEventArgs());
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            --CurrentIndex;
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            ++CurrentIndex;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
                CloseMe();
        }

        private void CurrentImage_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            CloseMe();
        }

        private void ImagesViewControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CurrentImage.MaxHeight = e.NewSize.Height;
            CurrentImage.MaxWidth = e.NewSize.Width;
        }

        private async void CloseMe()
        {
            var isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";
            if (isMobile)
                await StatusBar.GetForCurrentView().ShowAsync();

            Shell.HamburgerMenu.IsFullScreen = false;
            Close(this, new ImageViewCloseEventArgs());
        }
    }

    public class ImageViewCloseEventArgs
    { }
}
