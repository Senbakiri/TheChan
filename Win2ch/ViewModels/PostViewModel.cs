using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Models;
using Win2ch.Common;

namespace Win2ch.ViewModels { 
    public class PostViewModel : PropertyChangedBase {
        private Post post;
        private bool showReplies = true;
        private bool showPostPosition = true;
        private int position;
        private PostForeground foreground;
        private string foregroundString;
        private int repliesCount;
        private bool areRepliesVisible;
        private bool isTextSelectionEnabled;

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

        public PostForeground Foreground {
            get { return this.foreground; }
            set {
                this.foreground = value;
                NotifyOfPropertyChange();
                ForegroundString = this.foreground.ToString();
            }
        }

        public string ForegroundString {
            get { return this.foregroundString; }
            private set {
                if (value == this.foregroundString)
                    return;
                this.foregroundString = value;
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

        public ObservableCollection<PostViewModel> Replies { get; } = new ObservableCollection<PostViewModel>(); 
    }
}