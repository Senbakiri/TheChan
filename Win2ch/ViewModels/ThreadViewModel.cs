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
using HtmlAgilityPack;
using Win2ch.Common;
using Win2ch.Common.Core;
using Win2ch.Common.UI;
using Win2ch.Extensions;
using Win2ch.Services.Storage;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class ThreadViewModel : Tab {
        private bool isHighlighting;
        private int highlightingStart;
        private bool isInFavorites;
        private string postText;

        public ThreadViewModel(IBoard board,
                               IShell shell,
                               IAttachmentViewer attachmentViewer,
                               FavoriteThreadsService favoriteThreadsService,
                               RecentThreadsService recentThreadsService) {
            Board = board;
            Shell = shell;
            AttachmentViewer = attachmentViewer;
            FavoriteThreadsService = favoriteThreadsService;
            RecentThreadsService = recentThreadsService;
            Posts = new ObservableCollection<PostViewModel>();
            PostInfo = new PostInfo();
        }

        private IShell Shell { get; }
        private IBoard Board { get; }
        private IAttachmentViewer AttachmentViewer { get; }
        private FavoriteThreadsService FavoriteThreadsService { get; }
        private RecentThreadsService RecentThreadsService { get; }
        private ICanScrollToItem<PostViewModel> PostScroll { get; set; } 
        private IReplyDisplay ReplyDisplay { get; set; }
        private ThreadLink Link { get; set; }
        private ThreadInfo ThreadInfo { get; set; }
        private PostInfo PostInfo { get; }
        public ObservableCollection<PostViewModel> Posts { get; }

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

                if (ThreadInfo != null) {
                    ThreadInfo.LastReadPostPosition = HighlightingStart - 1;
                    ThreadInfo.UnreadPosts = ThreadInfo.LastLoadedPostPosition - ThreadInfo.LastReadPostPosition;
                }
            }
        }

        public bool IsInFavorites {
            get { return this.isInFavorites; }
            private set {
                if (value == this.isInFavorites)
                    return;
                this.isInFavorites = value;
                NotifyOfPropertyChange();
            }
        }

        public string PostText {
            get { return this.postText; }
            set {
                if (value == this.postText)
                    return;
                this.postText = value;
                PostInfo.Text = value;
                NotifyOfPropertyChange();
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
            var nav = (ThreadNavigation) parameter;
            if (string.IsNullOrWhiteSpace(nav.BoardId)) 
                throw new ArgumentException();
            
            Link = new ThreadLink(nav.BoardId, nav.ThreadNumber);
            DisplayName = $"/{nav.BoardId}/ - {nav.ThreadNumber}";
            IsLoading = true;
            Shell.LoadingInfo.InProgress(GetLocalizationString("Loading"));
            try {
                Thread thread = await Board.LoadThreadAsync(Link);
                ThreadInfo = FavoriteThreadsService.GetThreadInfoOrCreate(thread);
                IsInFavorites = FavoriteThreadsService.Items.Contains(ThreadInfo);
                await SaveThreadToRecentThreads();
                DisplayName = GetDisplayName(thread);
                Posts.Clear();
                FillPosts(thread.Posts);
                Shell.LoadingInfo.Success(GetLocalizationString("Loaded"));
                HandleScrolling(nav);
                HandleHighlighting(nav);
            } catch (Exception) {
                Shell.LoadingInfo.Error(GetLocalizationString("NotLoaded"), true, () => OnActivate(nav));
            }

            IsLoading = false;
        }

        private void HandleHighlighting(ThreadNavigation nav) {
            if (!nav.IsHighlightingNeeded)
                return;
            IsHighlighting = true;
            HighlightingStart = nav.HighlightingStart;
        }

        private void HandleScrolling(ThreadNavigation nav) {
            if (!nav.IsScrollingToPostNeeded)
                return;
            if (nav.PostPosition == 0)
                PostScroll?.ScrollToItem(Posts.FirstOrDefault(p => p.Post.Number == nav.PostNumber));
            else if (nav.PostNumber == 0)
                PostScroll?.ScrollToItem(Posts.FirstOrDefault(p => p.Position == nav.PostPosition));
        }

        private async Task SaveThreadToRecentThreads() {
            if (RecentThreadsService.Items.Contains(ThreadInfo))
                RecentThreadsService.Items.Remove(ThreadInfo);
            RecentThreadsService.Items.Add(ThreadInfo);
            await RecentThreadsService.Save();
        }

        private static string GetDisplayName(Thread thread) {
            Post first = thread.Posts.First();
            string text = string.IsNullOrWhiteSpace(first.Subject) ? first.Text : first.Subject;
            var document = new HtmlDocument();
            document.LoadHtml(text.Replace("<br>", "\n"));
            return document.DocumentNode.InnerText.Split('\n')[0];
        }

        public async void RefreshThread() {
            if (string.IsNullOrEmpty(Link?.BoardId))
                return;
            
            IsLoading = true;
            Shell.LoadingInfo.InProgress(GetLocalizationString("Refreshing"));
            try {
                Thread thread = await Board.LoadThreadAsync(Link, Posts.Count + 1);
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
            if (string.IsNullOrEmpty(Link?.BoardId))
                return false;
            
            IsLoading = true;
            Thread thread = await Board.LoadThreadAsync(Link, Posts.Count + 1);
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

            SetupEventsForPost(postViewModel);
            return postViewModel;
        }

        public void SetupEventsForPost(PostViewModel post) {
            post.RepliesDisplayingRequested += PostViewModelOnRepliesDisplayingRequested;
            post.PostDisplayingRequested += PostViewModelOnPostDisplayingRequested;
            post.ReplyDisplayingRequested += PostViewModelOnReplyDisplayingRequested;
            post.AttachmentOpeningRequested += PostOnAttachmentOpeningRequested;
        }

        private void PostOnAttachmentOpeningRequested(object sender, AttachmentOpeningRequestedEventArgs e) {
            AttachmentViewer.View(e.Attachment, Posts.SelectMany(vm => vm.Post.Attachments));
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

        public async void Favorite() {
            bool isThreadInFavorites = FavoriteThreadsService.Items.Contains(ThreadInfo);
            if (isThreadInFavorites)
                FavoriteThreadsService.Items.Remove(ThreadInfo);
            else
                FavoriteThreadsService.Items.Add(ThreadInfo);

            IsInFavorites = !isThreadInFavorites;
            await FavoriteThreadsService.Save();
        }

        protected override async void OnDeactivate(bool close) {
            if (close)
                await FavoriteThreadsService.Save();
        }

        public void ShowExtendedPostingPopup() {
            var viewModel = new ExtendedPostingViewModel(Shell, Board, PostInfo, Link);
            viewModel.PostInfoChanged += ExtendedPostingViewModelOnPostInfoChanged;
            Shell.ShowPopup(viewModel);
        }

        private void ExtendedPostingViewModelOnPostInfoChanged(object sender, PostInfoChangedEventArgs e) {
            PostInfo pi = PostInfo; // we could also use e.PostInfo, but it is the same thing because of passing it by reference
            PostText = pi.Text;
        }

        public async void SendPost() {
            IsLoading = true;
            Shell.LoadingInfo.InProgress("[Posting]");
            PostInfo postInfo = PostInfo.Clone();
            try {
                PostingResult result = await Board.PostAsync(postInfo, Link.BoardId, Link.ThreadNumber);
                if (!result.IsSuccessful)
                    throw new Exception(result.Error);
                Shell.LoadingInfo.Success("[Posted]");
                PostInfo.Clear();
                RefreshThread();
            }
            catch (Exception e) {
                Shell.LoadingInfo.Error(e.Message);
            }
            IsLoading = false;
        }
    }
}