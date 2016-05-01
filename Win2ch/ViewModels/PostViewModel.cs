using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Common;
using Core.Common.Links;
using Core.Models;
using Win2ch.Common;
using Win2ch.Views;

namespace Win2ch.ViewModels { 
    public class PostViewModel : PropertyChangedBase {
        private Post post;
        private bool showReplies = true;
        private bool showPostPosition = true;
        private int position;
        private int repliesCount;
        private bool areRepliesVisible;
        private bool isTextSelectionEnabled;
        private bool isInFavorites;

        public PostViewModel() {
            Replies.CollectionChanged += (s, e) => RepliesCount = Replies.Count;
        }

        public Post Post {
            get { return this.post; }
            set {
                if (Equals(value, this.post))
                    return;
                this.post = value;
                NotifyOfPropertyChange();
            }
        }

        public int Position {
            get { return this.position; }
            set {
                if (value == this.position)
                    return;
                this.position = value;
                NotifyOfPropertyChange();
            }
        }

        public bool ShowReplies {
            get { return this.showReplies; }
            set {
                if (value == this.showReplies)
                    return;
                this.showReplies = value;
                NotifyOfPropertyChange();
                AreRepliesVisible = ShowReplies && RepliesCount > 0;
            }
        }

        public bool ShowPostPosition {
            get { return this.showPostPosition; }
            set {
                if (value == this.showPostPosition)
                    return;
                this.showPostPosition = value;
                NotifyOfPropertyChange();
            }
        }

        public int RepliesCount {
            get { return this.repliesCount; }
            private set {
                if (value == this.repliesCount)
                    return;
                this.repliesCount = value;
                NotifyOfPropertyChange();
                AreRepliesVisible = ShowReplies && RepliesCount > 0;
            }
        }

        public bool AreRepliesVisible {
            get { return this.areRepliesVisible; }
            private set {
                if (value == this.areRepliesVisible)
                    return;
                this.areRepliesVisible = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsTextSelectionEnabled {
            get { return this.isTextSelectionEnabled; }
            set {
                if (value == this.isTextSelectionEnabled)
                    return;
                this.isTextSelectionEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsInFavorites {
            get { return this.isInFavorites; }
            set {
                if (value == this.isInFavorites)
                    return;
                this.isInFavorites = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedText { get; set; } = string.Empty;

        public event EventHandler RepliesDisplayingRequested;
        public event EventHandler<PostDisplayingRequestedEventArgs> PostDisplayingRequested;
        public event EventHandler<ReplyDisplayingEventArgs> ReplyDisplayingRequested;
        public event EventHandler<AttachmentOpeningRequestedEventArgs> AttachmentOpeningRequested; 

        public ObservableCollection<PostViewModel> Replies { get; } = new ObservableCollection<PostViewModel>();

        public void DisplayReplies() {
            RepliesDisplayingRequested?.Invoke(this, EventArgs.Empty);
        }

        public void DisplayPost(PostLink link) {
            PostDisplayingRequested?.Invoke(this, new PostDisplayingRequestedEventArgs(link));
        }

        public void RequestReplyDisplaying(ReplyDisplayingEventArgs replyDisplayingEventArgs) {
            ReplyDisplayingRequested?.Invoke(this, replyDisplayingEventArgs);
        }

        public void OpenAttachment(Attachment attachment) {
            AttachmentOpeningRequested?.Invoke(this, new AttachmentOpeningRequestedEventArgs(attachment));
        }
    }

    public class PostDisplayingRequestedEventArgs : EventArgs {
        public PostDisplayingRequestedEventArgs(PostLink link) {
            Link = link;
        }

        public PostLink Link { get; }
    }

    public class AttachmentOpeningRequestedEventArgs : EventArgs {
        public AttachmentOpeningRequestedEventArgs(Attachment attachment) {
            Attachment = attachment;
        }

        public Attachment Attachment { get; }
    }
}