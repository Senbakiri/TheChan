using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Win2ch.Controls;
using Win2ch.Models;
using Win2ch.Services.SettingsServices;

namespace Win2ch.Common {
    public class AttachmentViewer {

        private Attachment Attachment { get; }
        private IEnumerable<Attachment> AllAttachments { get; }
        private Panel Parent { get; }
        public ICanScrollToItem<Attachment> Scroller { get; set; }

        public AttachmentViewer(Attachment attachment,
                                Panel parent,
                                IEnumerable<Attachment> allAttachments) {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            Attachment = attachment;
            Parent = parent;
            AllAttachments = allAttachments;
        }

        public void Open() {
            switch (Attachment.Type) {
                case AttachmentType.Jpg:
                case AttachmentType.Png:
                case AttachmentType.Gif:
                    OpenImage();
                    break;
                case AttachmentType.WebM:
                    OpenWebM();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OpenImage() {
            if (Attachment.Type == AttachmentType.WebM)
                throw new ArgumentException("Type must be either jpg, or png, or gif");

            var viewer = new ImagesViewer(Attachment, AllAttachments.ToList());
            viewer.Closed += ImagesViewerClosed;
            Parent.Children.Clear();
            Parent.Children.Add(viewer);
        }

        private void ImagesViewerClosed(object sender, ImagesViewerClosedEventArgs e) {
            Parent.Children.Clear();
            if (SettingsService.Instance.ScrollToPostWithImageAfterViewingImage)
                Scroller?.ScrollToItem(e.LastImage);
        }

        private void OpenWebM() {
            if (Attachment.Type != AttachmentType.WebM)
                throw new ArgumentException("Type must be webm");
            var player = new WebMPlayer(Attachment);
            Parent.Children.Clear();
            Parent.Children.Add(player);

        }
    }
}