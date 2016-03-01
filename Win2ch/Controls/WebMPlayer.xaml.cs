using System;
using FFmpegInterop;
using Win2ch.Models;
using Win2ch.Services;

namespace Win2ch.Controls {
    public sealed partial class WebMPlayer {
        public Attachment Attachment { get; }

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
