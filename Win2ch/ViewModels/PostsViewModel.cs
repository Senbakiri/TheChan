using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Common;
using Core.Models;
using Core.Operations;
using Win2ch.Common;
using Win2ch.Extensions;

namespace Win2ch.ViewModels {
    public class PostsViewModel : PropertyChangedBase {
        private object popupContent;
        private bool canGoToPost;
        private bool canGoToThread;

        public PostsViewModel(IEnumerable<PostViewModel> posts) {
            Posts = new ObservableCollection<PostViewModel>(posts.Select(CreatePostViewModel));
        }

        public PostsViewModel(IShell shell, IBoard board, ThreadViewModel source, long postNumber) {
            Posts = new ObservableCollection<PostViewModel>();
            Board = board;
            Shell = shell;
            LoadPost(source, postNumber);
        }

        private IBoard Board { get; }

        private IShell Shell { get; }

        public ObservableCollection<PostViewModel> Posts { get; }

        public bool CanGoToPost {
            get { return this.canGoToPost; }
            private set {
                if (value == this.canGoToPost)
                    return;
                this.canGoToPost = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanGoToThread {
            get { return this.canGoToThread; }
            private set {
                if (value == this.canGoToThread)
                    return;
                this.canGoToThread = value;
                NotifyOfPropertyChange();
            }
        }

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

        private async void LoadPost(ThreadViewModel source, long postNumber) {
            PostViewModel existingPost = source.Posts.FirstOrDefault(p => p.Post.Number == postNumber);
            if (existingPost != null) {
                CanGoToPost = true;
                Posts.Add(CreatePostViewModel(existingPost));
                return;
            }

            IGetPostOperation operation = Board.Operations.GetPost();
            operation.BoardId = source.Link.BoardId;
            operation.PostNumber = postNumber;
            try {
                Shell.LoadingInfo.InProgress(null);
                Post post = await operation.ExecuteAsync();
                PostViewModel viewModel = CreateViewModelForPost(post);
                Posts.Add(viewModel);
                CanGoToPost = true;
                CanGoToThread = true;
                Shell.LoadingInfo.Success(null);
            } catch {
                Shell.LoadingInfo.Error(null);
            }
            
        }

        private PostViewModel CreateViewModelForPost(Post post) {
            var viewModel = new PostViewModel {
                Post = post,
                ShowReplies = false,
                ShowPostPosition = false,
                IsTextSelectionEnabled = true,
                Foreground = PostForeground.Gray,
            };

            return viewModel;
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