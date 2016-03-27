using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Models;
using Win2ch.Common;
using Win2ch.Extensions;

namespace Win2ch.ViewModels {
    public class PostsViewModel : PropertyChangedBase {
        private object popupContent;

        public PostsViewModel(IEnumerable<PostViewModel> posts) {
            Posts = new ObservableCollection<PostViewModel>(posts.Select(CreatePostViewModel));
        }

        public ObservableCollection<PostViewModel> Posts { get; }

        public event EventHandler Close;

        public object PopupContent {
            get { return this.popupContent; }
            private set {
                if (Equals(value, this.popupContent))
                    return;
                this.popupContent = value;
                NotifyOfPropertyChange();
            }
        }

        private PostViewModel CreatePostViewModel(PostViewModel old) {
            var postViewModel = new PostViewModel {
                ShowReplies = true,
                ShowPostPosition = true,
                Position = old.Position,
                Foreground = PostForeground.Contrast,
                IsTextSelectionEnabled = true,
                Post = old.Post,
            };

            postViewModel.Replies.AddRange(old.Replies);
            postViewModel.RepliesDisplayRequested += PostViewModelOnRepliesDisplayRequested;
            return postViewModel;
        }

        private void PostViewModelOnRepliesDisplayRequested(object sender, EventArgs eventArgs) {
            var viewModel = new PostsViewModel(((PostViewModel) sender).Replies);
            viewModel.Close += (s, e) => PopupContent = null;
            PopupContent = viewModel;
        }

        internal void CloseDown() {
            Close?.Invoke(this, EventArgs.Empty);
        }
    }
}