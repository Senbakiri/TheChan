using System;
using Caliburn.Micro;
using Core.Common;

namespace Win2ch.ViewModels {
    public class ExtendedPostingViewModel : PropertyChangedBase {
        private string postText;

        public ExtendedPostingViewModel(PostInfo postInfo) {
            PostInfo = postInfo;
            PostText = PostInfo.Text;
        }
    
        private PostInfo PostInfo { get; }
        public event EventHandler<PostInfoChangedEventArgs> PostInfoChanged; 

        public string PostText {
            get { return this.postText; }
            set {
                if (value == this.postText)
                    return;
                this.postText = value;
                PostInfo.Text = value;
                NotifyOfPropertyChange();
                NotifyOfPostInfoChange();
            }
        }

        private void NotifyOfPostInfoChange() {
            PostInfoChanged?.Invoke(this, new PostInfoChangedEventArgs(PostInfo));
        }
    }

    public class PostInfoChangedEventArgs : EventArgs {
        public PostInfoChangedEventArgs(PostInfo postInfo) {
            PostInfo = postInfo;
        }

        public PostInfo PostInfo { get; }
    }
}