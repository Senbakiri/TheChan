using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Utils;
using TheChan.Common.UI;
using TheChan.ViewModels;

namespace TheChan.Views {
    public sealed partial class ExtendedPostingView {
        public ExtendedPostingView(IShell shell) {
            Shell = shell;
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ExtendedPostingViewModel;
        }

        private IShell Shell { get; }
        public ExtendedPostingViewModel ViewModel { get; set; }

        private void ExtendedPostingView_OnLoaded(object sender, RoutedEventArgs e) {
            if (DeviceUtils.CurrentDeviceFamily != DeviceUtils.DeviceFamilies.Mobile) {
                this.Window.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        private void ExtendedPostingView_OnDragOver(object sender, DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;
            if (e.DragUIOverride == null)
                return;
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }

        private async void ExtendedPostingView_OnDrop(object sender, DragEventArgs e) {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
                return;

            var items = await e.DataView.GetStorageItemsAsync();
            foreach (IStorageFile file in items.OfType<IStorageFile>()) {
                await ViewModel.Attach(file);

            }
        }

        private async void TextBox_OnPaste(object sender, TextControlPasteEventArgs e) {
            DataPackageView content = Clipboard.GetContent();
            List<string> formats = content.AvailableFormats.ToList();
            if (!formats.Contains(StandardDataFormats.Bitmap))
                return;

            RandomAccessStreamReference bitmap = await Clipboard.GetContent().GetBitmapAsync();
            await ViewModel.AttachPastedFile(await BitmapToPng(bitmap));
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
    }
}
