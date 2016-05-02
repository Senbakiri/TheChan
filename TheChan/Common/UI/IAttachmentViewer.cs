using System.Collections.Generic;
using Core.Models;

namespace TheChan.Common.UI {
    public interface IAttachmentViewer {
        void View(Attachment currentAttachment, IEnumerable<Attachment> allAttachments);
    }
}