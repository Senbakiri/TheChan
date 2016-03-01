using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FFmpegInterop;
using Win2ch.Models;
using Win2ch.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Controls {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebMPlayer : Page {
        public Attachment Attachment { get; private set; }

        public WebMPlayer(Attachment attachment) {
            Attachment = attachment;
            InitializeComponent();
            CreateStream();
        }

        private async void CreateStream() {
            var file = await CacheService.Instance.DownloadAndCacheItem(Attachment.Url, CacheItemType.Video, Attachment.Name);
            var stream = await file.OpenReadAsync();
            var source = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(stream, true, true);
            var mss = source.GetMediaStreamSource();
            mss.BufferTime = new TimeSpan(0);
            MediaElement.SetMediaStreamSource(mss);
        }
    }
}
