using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Core.Common;
using Core.Common.Links;
using Core.Models;
using Core.Operations;
using Win2ch.Common;
using Win2ch.Extensions;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class ThreadViewModel : Tab {
        private bool isHighlighting;
        private int highlightingStart;

        public ThreadViewModel(IBoard board, IShell shell) {
            Board = board;
            Shell = shell;
            Operation = Board.Operations.LoadThread();
            Posts = new ObservableCollection<PostViewModel>();
        }

        private IShell Shell { get; }
        private IBoard Board { get; }
        private ILoadThreadOperation Operation { get; }
        private ICanScrollToItem<PostViewModel> PostScroll { get; set; } 
        private IReplyDisplay ReplyDisplay { get; set; }
        public ObservableCollection<PostViewModel> Posts { get; }
        public ThreadLink Link { get; set; }

        public bool IsHighlighting {
            get { return this.isHighlighting; }
            private set {
                if (value == this.isHighlighting)
                    return;
                this.isHighlighting = value;
                NotifyOfPropertyChange();
                UpdateBadge();
            }
        }

        public int HighlightingStart {
            get { return this.highlightingStart; }
            set {
                if (value == this.highlightingStart)
                    return;
                this.highlightingStart = value;
                NotifyOfPropertyChange();
                UpdateBadge();
            }
        }

        protected override void OnViewAttached(object view, object context) {
            var postScroll = view as ICanScrollToItem<PostViewModel>;
            if (postScroll != null)
                PostScroll = postScroll;

            var replyDisplay = view as IReplyDisplay;
            if (replyDisplay != null)
                ReplyDisplay = replyDisplay;
        }

        protected override async void OnActivate(object parameter = null) {
            if (parameter == null)
                return;
            var navigation = (ThreadNavigation) parameter;
            if (string.IsNullOrWhiteSpace(navigation.BoardId))
                throw new ArgumentException();

            Operation.BoardId = navigation.BoardId;
            Operation.ThreadNumber = navigation.ThreadNumber;
            Link = new ThreadLink(navigation.BoardId, navigation.ThreadNumber);
            DisplayName = $"/{navigation.BoardId}/ - {navigation.ThreadNumber}";
            IsLoading = true;
            Shell.LoadingInfo.InProgress(GetLocalizationString("Loading"));
            try {
                Thread thread = await Operation.ExecuteAsync();
                DisplayName = GetDisplayName(thread);
                Posts.Clear();
                FillPosts(thread.Posts);
                Shell.LoadingInfo.Success(GetLocalizationString("Loaded"));
                if (navigation.IsScrollingToPostNeeded) {
                    PostScroll?.ScrollToItem(Posts.FirstOrDefault(p => p.Post.Number == navigation.PostNumber));
                }
            } catch (Exception) {
                Shell.LoadingInfo.Error(GetLocalizationString("NotLoaded"), true, () => OnActivate(navigation));
            }

            IsLoading = false;
        }

        private static string GetDisplayName(Thread thread) {
            Post first = thread.Posts.First();
            return string.IsNullOrWhiteSpace(first.Subject) ? first.Text : first.Subject;
        }

        public async void RefreshThread() {
            if (string.IsNullOrEmpty(Operation.BoardId))
                return;

            Operation.FromPosition = Posts.Count + 1;
            IsLoading = true;
            Shell.LoadingInfo.InProgress(GetLocalizationString("Refreshing"));
            try {
                Thread thread = await Operation.ExecuteAsync();
                int count = Posts.Count;
                FillPosts(thread.Posts);
                if (HighlightingStart == 0)
                    HighlightingStart = count + 1;
                if (thread.Posts.Count > 0) {
                    Shell.LoadingInfo.Success($"{GetLocalizationString("Refreshed.NewPosts")}: {thread.Posts.Count}");
                    IsHighlighting = true;
                } else {
                    Shell.LoadingInfo.Success(GetLocalizationString("Refreshed.NoNewPosts"));
                }
            } catch (Exception) {
                Shell.LoadingInfo.Error(GetLocalizationString("NotRefreshed"));
            }

            UpdateBadge();
            IsLoading = false;
        }

        public async Task<bool> Update() {
            if (string.IsNullOrEmpty(Operation.BoardId))
                return false;

            Operation.FromPosition = Posts.Count + 1;
            IsLoading = true;
            Thread thread = await Operation.ExecuteAsync();
            IsLoading = false;
            int count = Posts.Count;
            FillPosts(thread.Posts);

            if (HighlightingStart == 0)
                HighlightingStart = count + 1;

            if (thread.Posts.Count > 0)
                IsHighlighting = true;
            UpdateBadge();
            return thread.Posts.Count > 0;
        }

        private void FillPosts(IEnumerable<Post> posts) {
            var newPosts = new List<PostViewModel>();
            int offset = Posts.Count + 1;
            newPosts.AddRange(posts.Select((post, i) => CreatePostViewModel(post, offset + i)));
            ProcessReplies(Posts, newPosts);
            Posts.AddRange(newPosts);
        }

        private PostViewModel CreatePostViewModel(Post post, int position) {
            var postViewModel = new PostViewModel {
                Position = position,
                Post = post,
                IsTextSelectionEnabled = true,
                ShowReplies = true,
                ShowPostPosition = true,
            };

            postViewModel.RepliesDisplayingRequested += PostViewModelOnRepliesDisplayingRequested;
            postViewModel.PostDisplayingRequested += PostViewModelOnPostDisplayingRequested;
            postViewModel.ReplyDisplayingRequested += PostViewModelOnReplyDisplayingRequested;
            return postViewModel;
        }

        public void SetupEventsForPost(PostViewModel post) {
            post.RepliesDisplayingRequested += PostViewModelOnRepliesDisplayingRequested;
            post.PostDisplayingRequested += PostViewModelOnPostDisplayingRequested;
            post.ReplyDisplayingRequested += PostViewModelOnReplyDisplayingRequested;
        }

        private void PostViewModelOnRepliesDisplayingRequested(object sender, EventArgs eventArgs) {
            var post = (PostViewModel) sender;
            var viewModel = new PostsViewModel(Shell, Board, Link.BoardId, Posts.ToList().AsReadOnly(), post.Replies);
            viewModel.Close += (s, e) => Shell.HidePopup();
            viewModel.NavigateToPost += PostsViewModelOnNavigateToPost;
            viewModel.NavigateToThread += PostsViewModelOnNavigateToThread;
            Shell.ShowPopup(viewModel);
        }

        private void PostViewModelOnPostDisplayingRequested(object sender, PostDisplayingRequestedEventArgs e) {
            var viewModel = new PostsViewModel(Shell, Board, e.Link.BoardId, Posts.ToList().AsReadOnly(), e.Link.PostNumber);
            viewModel.Close += (s, _) => Shell.HidePopup();
            viewModel.NavigateToPost += PostsViewModelOnNavigateToPost;
            viewModel.NavigateToThread += PostsViewModelOnNavigateToThread;
            Shell.ShowPopup(viewModel);
        }

        private void PostViewModelOnReplyDisplayingRequested(object sender, ReplyDisplayingEventArgs replyDisplayingEventArgs) {
            ReplyDisplay?.DisplayReply(replyDisplayingEventArgs);
        }

        private void PostsViewModelOnNavigateToThread(object sender, NavigateToThreadEventArgs e) {
            if (e.Link.BoardId != Link.BoardId || e.Link.ThreadNumber != Link.ThreadNumber) {
                Shell.HidePopup();
                Shell.Navigate<ThreadViewModel>(ThreadNavigation.NavigateToThread(e.Link.BoardId, e.Link.ThreadNumber));
            }
        }

        private void PostsViewModelOnNavigateToPost(object sender, NavigateToPostEventArgs e) {
            Shell.HidePopup();
            if (e.Link.BoardId != Link.BoardId || e.Link.ThreadNumber != Link.ThreadNumber) {
                Shell.Navigate<ThreadViewModel>(
                    ThreadNavigation
                        .NavigateToThread(e.Link.BoardId, e.Link.ThreadNumber)
                        .ScrollToPost(e.Link.PostNumber));
            } else {
                PostScroll?.ScrollToItem(Posts.FirstOrDefault(p => p.Post.Number == e.Link.PostNumber));
            }
        }

        private static void ProcessReplies(IEnumerable<PostViewModel> existingPosts, IList<PostViewModel> newPosts) {
            Dictionary<long, IList<PostViewModel>> repliedPosts = FindRepliedPosts(newPosts);
            IList<PostViewModel> searchSource = existingPosts.Concat(newPosts).ToList();
            foreach (KeyValuePair<long, IList<PostViewModel>> reply in repliedPosts) {
                long postNumber = reply.Key;
                IList<PostViewModel> replies = reply.Value;
                PostViewModel post = searchSource.FirstOrDefault(p => p.Post.Number == postNumber);
                post?.Replies.AddRange(replies);
            }
        }

        private static Dictionary<long, IList<PostViewModel>> FindRepliedPosts(IEnumerable<PostViewModel> posts) {
            var result = new Dictionary<long, IList<PostViewModel>>();
            var replyRegex = new Regex(@">>(\d+)");
            foreach (PostViewModel post in posts) {
                MatchCollection matches = replyRegex.Matches(post.Post.Text);
                foreach (Match match in matches) {
                    long postNumber = Convert.ToInt64(match.Groups[1].Captures[0].Value);
                    if (!result.ContainsKey(postNumber))
                        result.Add(postNumber, new List<PostViewModel>());
                    result[postNumber].Add(post);
                }
            }

            return result;
        }

        private void UpdateBadge() {
            BadgeContent = IsHighlighting && HighlightingStart <= Posts.Count
                ? $"+{Posts.Count - HighlightingStart + 1}"
                : "";
        }

        public void CopyLink() {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Board.UrlService.GetUrlForLink(Link).AbsoluteUri);
            Clipboard.SetContent(dataPackage);
        }
    }
}