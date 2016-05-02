using System.Collections.Generic;
using Core.Models;
using TheChan.Common.UI;
using TheChan.ViewModels;

namespace TheChan.Common {
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
            var view = new ImagesViewModel(CurrentAttachment, AllAttachments);
            view.ClosingRequested += (s, e) => Shell.HidePopup();
            Shell.ShowPopup(view);
        }
    }
}