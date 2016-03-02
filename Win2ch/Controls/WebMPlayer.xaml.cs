using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FFmpegInterop;
using Win2ch.Annotations;
using Win2ch.Common;
using Win2ch.Models;
using Win2ch.Services;
using Win2ch.Services.SettingsServices;

namespace Win2ch.Controls {
    public sealed partial class WebMPlayer : INotifyPropertyChanged {
        private bool _IsLoading;
        private bool _IsWebmEnabled;
        public Attachment Attachment { get; }

        public bool IsLoading {
            get { return _IsLoading; }
            private set {
                if (value == _IsLoading)
                    return;
                _IsLoading = value;
                RaisePropertyChanged();
            }
        }

        public bool IsWebmEnabled {
            get { return _IsWebmEnabled; }
            private set {
                if (value == _IsWebmEnabled)
                    return;
                _IsWebmEnabled = value;
                RaisePropertyChanged();
            }
        }

        public WebMPlayer(Attachment attachment) {
            Attachment = attachment;
            InitializeComponent();
            IsWebmEnabled = SettingsService.Instance.IsWebmEnabled;
            if (IsWebmEnabled)
                CreateStream();
        }

        private async void CreateStream() {
            try {
                IsLoading = true;
                var file = await CacheService.Instance.DownloadAndCacheAttachment(Attachment);
                var stream = await file.OpenReadAsync();
                var source = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(stream, true, true);
                var mss = source.GetMediaStreamSource();
                mss.BufferTime = new TimeSpan(0);
                MediaElement.SetMediaStreamSource(mss);
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось загрузить видео");
            }

            IsLoading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
