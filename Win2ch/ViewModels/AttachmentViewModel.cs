using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Win2ch.ViewModels {
    public class AttachmentViewModel {
        public BitmapImage Image { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public IRandomAccessStreamReference Reference { get; set; }
    }
}