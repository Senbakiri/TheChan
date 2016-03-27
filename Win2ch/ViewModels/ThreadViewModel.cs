using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Core.Common;
using Core.Models;
using Core.Operations;
using Makaba.Links;
using Win2ch.Common;
using Win2ch.Extensions;

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
            ProcessAnswers(Posts, newPosts);
            Posts.AddRange(newPosts);
        }

        private PostViewModel CreatePostViewModel(Post post, int position) {
            var postViewModel = new PostViewModel {
                Foreground = PostForeground.Gray,
                Position = position,
                Post = post,
                IsTextSelectionEnabled = true,
                ShowReplies = true,
                ShowPostPosition = true,
            };

            postViewModel.RepliesDisplayRequested += PostViewModelOnRepliesDisplayRequested;
            postViewModel.PostDisplayRequested += PostViewModelOnPostDisplayRequested;
            return postViewModel;
        }

        private void PostViewModelOnRepliesDisplayRequested(object sender, EventArgs eventArgs) {
            var post = (PostViewModel) sender;
            var viewModel = new PostsViewModel(post.Replies);
            viewModel.Close += (s, e) => Shell.HidePopup();Shell.ShowPopup(viewModel);
        }

        private void PostViewModelOnPostDisplayRequested(object sender, PostDisplayRequestedEventArgs e) {
            var viewModel = new PostsViewModel(Shell, Board, this, e.PostNumber);
            viewModel.Close += (s, _) => Shell.HidePopup();
            Shell.ShowPopup(viewModel);
        }

        private static void ProcessAnswers(IEnumerable<PostViewModel> existingPosts, IList<PostViewModel> newPosts) {
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
            dataPackage.SetText(Link.GetUrl());
            Clipboard.SetContent(dataPackage);
        }
    }
}