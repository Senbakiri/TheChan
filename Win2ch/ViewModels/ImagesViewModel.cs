using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Core.Models;

namespace Win2ch.ViewModels {
    public class ImagesViewModel : PropertyChangedBase {
        private Attachment currentAttachment;
        private int currentAttachmentIndex = -1;
        public ObservableCollection<Attachment> Attachments { get; }
        public event EventHandler<ImagesViewClosingRequestedEventArgs> ClosingRequested;

        public Attachment CurrentAttachment {
            get { return this.currentAttachment; }
            private set {
                if (Equals(value, this.currentAttachment))
                    return;
                this.currentAttachment = value;
                CurrentAttachmentIndex = Attachments.IndexOf(value);
                NotifyOfPropertyChange();
            }
        }

        public int CurrentAttachmentIndex {
            get { return this.currentAttachmentIndex; }
            set {
                if (value == this.currentAttachmentIndex)
                    return;
                this.currentAttachmentIndex = value;
                CurrentAttachment = Attachments[value];
                NotifyOfPropertyChange();
            }
        }

        public ImagesViewModel(Attachment currentAttachment, IEnumerable<Attachment> allAttachments) {
            Attachments = new ObservableCollection<Attachment>(allAttachments);
            CurrentAttachment = currentAttachment;
        }


        public void RequestClosing() {
            ClosingRequested?.Invoke(this, new ImagesViewClosingRequestedEventArgs(CurrentAttachment));
        }
    }

    public class ImagesViewClosingRequestedEventArgs : EventArgs {
        public ImagesViewClosingRequestedEventArgs(Attachment lastAttachment) {
            LastAttachment = lastAttachment;
        }

        public Attachment LastAttachment { get; }
    }
}