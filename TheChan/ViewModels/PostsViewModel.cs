using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Common;
using Core.Common.Links;
using Core.Models;
using TheChan.Common.UI;
using TheChan.Extensions;

namespace TheChan.ViewModels {
    public class PostsViewModel : PropertyChangedBase {
        private object popupContent;
        private bool canGoToPost;
        private bool canGoToThread;

        public PostsViewModel(IShell shell, IBoard board, string boardId, IList<PostViewModel> threadPosts, IEnumerable<PostViewModel> posts) {
            Posts = new ObservableCollection<PostViewModel>(posts.Select(CreatePostViewModel));
            BoardId = boardId;
            Board = board;
            ThreadPosts = threadPosts;
            Shell = shell;
        }

        public PostsViewModel(IShell shell, IBoard board, string boardId, IList<PostViewModel> threadPosts, long postNumber) {
            Posts = new ObservableCollection<PostViewModel>();
            Board = board;
            Shell = shell;
            BoardId = boardId;
            ThreadPosts = threadPosts;
            LoadPost(postNumber);
        }

        private string BoardId { get; }

        private IBoard Board { get; }

        private IShell Shell { get; }

        private IList<PostViewModel> ThreadPosts { get; } 

        public ObservableCollection<PostViewModel> Posts { get; }

        public event EventHandler<NavigateToPostEventArgs> NavigateToPost;

        public event EventHandler<NavigateToThreadEventArgs> NavigateToThread; 

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
                IsTextSelectionEnabled = true,
                Post = old.Post,
            };

            postViewModel.Replies.AddRange(old.Replies);
            postViewModel.RepliesDisplayingRequested += PostViewModelOnRepliesDisplayingRequested;
            postViewModel.PostDisplayingRequested += PostViewModelOnPostDisplayingRequested;
            return postViewModel;
        }

        private async void LoadPost(long postNumber) {
            PostViewModel existingPost = ThreadPosts.FirstOrDefault(p => p.Post.Number == postNumber);
            if (existingPost != null) {
                CanGoToPost = true;
                Posts.Add(CreatePostViewModel(existingPost));
                return;
            }
            
            try {
                Shell.LoadingInfo.InProgress("");
                Post post = await Board.LoadPostAsync(BoardId, postNumber);
                PostViewModel viewModel = CreateViewModelForPost(post);
                Posts.Add(viewModel);
                CanGoToPost = true;
                CanGoToThread = true;
                Shell.LoadingInfo.Success("");
            } catch {
                Shell.LoadingInfo.Error("");
                CloseDown();
            }
            
        }

        private PostViewModel CreateViewModelForPost(Post post) {
            var viewModel = new PostViewModel {
                Post = post,
                ShowReplies = false,
                ShowPostPosition = false,
                IsTextSelectionEnabled = true,
            };

            viewModel.PostDisplayingRequested += PostViewModelOnPostDisplayingRequested;
            return viewModel;
        }

        private void PostViewModelOnRepliesDisplayingRequested(object sender, EventArgs eventArgs) {
            var viewModel = new PostsViewModel(Shell, Board, BoardId, ThreadPosts, ((PostViewModel) sender).Replies);
            viewModel.Close += (s, e) => PopupContent = null;
            viewModel.NavigateToPost += NavigateToPost;
            viewModel.NavigateToThread += NavigateToThread;
            PopupContent = viewModel;
        }

        private void PostViewModelOnPostDisplayingRequested(object sender, PostDisplayingRequestedEventArgs e) {
            var viewModel = new PostsViewModel(Shell, Board, e.Link.BoardId, ThreadPosts, e.Link.PostNumber);
            viewModel.Close += (s, _) => PopupContent = null;
            viewModel.NavigateToPost += NavigateToPost;
            viewModel.NavigateToThread += NavigateToThread;
            PopupContent = viewModel;
        }

        internal void CloseDown() {
            Close?.Invoke(this, EventArgs.Empty);
        }

        public void GoToPost() {
            Post post = Posts.First().Post;
            NavigateToPost?.Invoke(this, new NavigateToPostEventArgs(new PostLink(BoardId, post.ParentNumber, post.Number)));
        }

        public void GoToThread() {
            Post post = Posts.First().Post;
            long num = post.ParentNumber == 0 ? post.Number : post.ParentNumber;
            NavigateToThread?.Invoke(this, new NavigateToThreadEventArgs(new ThreadLink(BoardId, num)));

        }
    }

    public class NavigateToThreadEventArgs : EventArgs {
        public NavigateToThreadEventArgs(ThreadLink link) {
            Link = link;
        }

        public ThreadLink Link { get; }
    }

    public class NavigateToPostEventArgs : EventArgs {
        public NavigateToPostEventArgs(PostLink link) {
            Link = link;
        }

        public PostLink Link { get; }
    }
}