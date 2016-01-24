using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Template10.Services.SerializationService;
using Win2ch.Models;
using Win2ch.Mvvm;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class ImagesViewModel : ViewModelBase {


        public ObservableCollection<BitmapImage> ImagesSources { get; }
        = new ObservableCollection<BitmapImage>();

        private List<ImageInfo> _AllImages;

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

        private BitmapImage _CurrentImage;
        public BitmapImage CurrentImage {
            get { return _CurrentImage; }
            set {
                _CurrentImage = value;
                RaisePropertyChanged();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) {

            var data = parameter as ImagesViewPageNavigationParameters;
            if (data == null)
                throw new ArgumentException();

            var isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";
            if (isMobile)
                await StatusBar.GetForCurrentView().HideAsync();

            AllImages = data.AllImages;
            CurrentImage = ImagesSources.FirstOrDefault(im => im.UriSource.OriginalString.Equals(data.Image.Url));

            Shell.HamburgerMenu.IsFullScreen = true;
        }

    }

    public class ImagesViewPageNavigationParameters {
        public ImagesViewPageNavigationParameters(ImageInfo image, List<ImageInfo> allImages) {
            AllImages = allImages;
            Image = image;
        }

        public ImageInfo Image { get; }
        public List<ImageInfo> AllImages { get; }
    }
}
