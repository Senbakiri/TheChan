using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Win2ch.Models;
using Win2ch.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;

namespace Win2ch.Views {
    public class PostingPageNavigationInfo {
        public NewPostInfo PostInfo { get; set; }
        public Thread Thread { get; set; }
    }
    public sealed partial class PostingPage {
        public PostingViewModel ViewModel => DataContext as PostingViewModel;

        public PostingPage() {
            InitializeComponent();
            InputPane.GetForCurrentView().Showing += InputPane_OnShowing;
            InputPane.GetForCurrentView().Hiding += InputPane_OnHiding;
        }

        private void InputPane_OnHiding(InputPane sender, InputPaneVisibilityEventArgs args) {
            Grid.RowDefinitions[0].MaxHeight = double.PositiveInfinity;
        }

        private void InputPane_OnShowing(InputPane sender, InputPaneVisibilityEventArgs args) {
            var height = Grid.ActualHeight - 50 - args.OccludedRect.Height;
            Grid.RowDefinitions[0].MaxHeight = height >= 0 ? height : double.PositiveInfinity;
        }

        private void TextButton_OnClick(object sender, RoutedEventArgs e) {
            TextBox.Focus(FocusState.Programmatic);
        }

        private void AttachedImages_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.AttachedImages.Remove(e.ClickedItem as BitmapImage);
        }

        private void More_OnClick(object sender, RoutedEventArgs e) {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
            AttachImagesButton.Focus(FocusState.Programmatic);
        }

        private async void TextBox_OnPaste(object sender, TextControlPasteEventArgs e) {
            var content = Clipboard.GetContent();
            var formats = content.AvailableFormats.ToList();
            if (!formats.Contains(StandardDataFormats.Bitmap))
                return;

            var bitmap = await Clipboard.GetContent().GetBitmapAsync();
            await ViewModel.AttachImage(await BitmapToPng(bitmap));
            SplitView.IsPaneOpen = true;
        }

        private static async Task<IRandomAccessStreamReference> BitmapToPng(IRandomAccessStreamReference bitmap) {
            var rndAccessStreamWithContentType = await bitmap.OpenReadAsync();
            var decoder = await BitmapDecoder.CreateAsync(rndAccessStreamWithContentType);
            var pixels = await decoder.GetPixelDataAsync();
            var outStream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outStream);
            encoder.SetPixelData(decoder.BitmapPixelFormat, BitmapAlphaMode.Ignore,
              decoder.OrientedPixelWidth, decoder.OrientedPixelHeight,
              decoder.DpiX, decoder.DpiY,
              pixels.DetachPixelData());
            await encoder.FlushAsync();
            return RandomAccessStreamReference.CreateFromStream(outStream);
        }

        private void PostingPage_OnDragOver(object sender, DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;
            if (e.DragUIOverride == null)
                return;
            e.DragUIOverride.Caption = "Прикрепить изображение";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }

        private async void PostingPage_OnDrop(object sender, DragEventArgs e) {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
                return;

            var items = await e.DataView.GetStorageItemsAsync();
            await ViewModel.AttachImages(items.Cast<StorageFile>());
        }
    }
}
