using System;
using System.Collections.Generic;
using Core.Models;
using Win2ch.ViewModels;

namespace Win2ch.Common {
    public class AttachmentViewer : IAttachmentViewer {
        private IList<Attachment> AllAttachments { get; set; }
        private Attachment CurrentAttachment { get; set; }
        private IShell Shell { get; }

        public AttachmentViewer(IShell shell) {
            Shell = shell;
        }

        public void View(Attachment currentAttachment, IEnumerable<Attachment> allAttachments) {
            AllAttachments = new List<Attachment>(allAttachments);
            CurrentAttachment = currentAttachment;
            switch (currentAttachment.Type) {
                default:
                    OpenImage();
                    break;
            }
        }

        private void OpenImage() {
            Shell.ShowPopup(new ImagesViewModel(CurrentAttachment, AllAttachments));
        }
    }
}