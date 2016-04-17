using System.Collections.Generic;
using Core.Models;

namespace Win2ch.Common.UI {
    public interface IAttachmentViewer {
        void View(Attachment currentAttachment, IEnumerable<Attachment> allAttachments);
    }
}