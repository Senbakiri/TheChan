using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Win2ch.Annotations;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.ViewModels;
using Win2ch.Views;

namespace Win2ch.Controls {
    public sealed partial class PostViewer : INotifyPropertyChanged {
        private bool _IsLoading;
        private bool _CanGoToPost;
        private bool _CanGoToThread;

        public bool IsLoading {
            get { return _IsLoading; }
            set {
                if (value == _IsLoading)
                    return;
                _IsLoading = value;
                RaisePropertyChanged();
            }
        }

        public bool CanGoToPost {
            get { return _CanGoToPost; }
            set {
                if (value == _CanGoToPost)
                    return;
                _CanGoToPost = value;
                RaisePropertyChanged();
            }
        }

        public bool CanGoToThread {
            get { return _CanGoToThread; }
            set {
                if (value == _CanGoToThread)
                    return;
                _CanGoToThread = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Post> Posts { get; }
        public event Action<object> Close;
        public event PostControl.ImageClickEventHandler ImageClick = delegate { };
        public event PostControl.PostReplyEventHandler Reply = delegate { };
        public event EventHandler<ReplyShowEventArgs> ReplyShowRequested = delegate { };
        public event PropertyChangedEventHandler PropertyChanged;
        private const int ManipulationAmountToClose = 100;
        private Thread SourceThread { get; }
        private int ThreadNum { get; set; }

        public PostViewer(Thread thread, IEnumerable<Post> posts) {
            Posts = new ObservableCollection<Post>(posts);
            SourceThread = thread;
            InitializeComponent();
        }

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PostViewer(Thread sourceThread, int threadNum, int postNum) {
            InitializeComponent();
            Posts = new ObservableCollection<Post>();
            SourceThread = sourceThread;
            Post post = null;
            ThreadNum = threadNum;
            if (threadNum == sourceThread.Num)
                post = sourceThread.Posts.FirstOrDefault(p => p.Num == postNum);

            if (post == null || threadNum != sourceThread.Num)
                TryToLoadPost(postNum);
            else {
                CanGoToPost = true;
                Posts.Add(post);
            }
        }

        private async void TryToLoadPost(int postNum) {
            IsLoading = true;
            const string errorMessage = "Не удалось получить пост";

            try {
                var post = await SourceThread.Board.GetPost(postNum);
                Thread.FillPosts(new List<Post> { post }, SourceThread.Board);
                Posts.Add(post);
                CanGoToThread = true;
            } catch (COMException e) {
                await Utils.ShowConnectionError(e, errorMessage);
                Close?.Invoke(this);
            } catch (HttpException e) {
                await Utils.ShowHttpError(e, errorMessage);
                Close?.Invoke(this);
            } catch (ApiException e) {
                await Utils.ShowOtherError(e, errorMessage);
                Close?.Invoke(this);
            }

            IsLoading = false;
            CanGoToPost = true;
        }

        private void PostControl_OnRepliesListShowRequested(Post post) {
            var control = new PostViewer(SourceThread, post.Replies);
            control.Close += s => Root.Children.Remove((UIElement)s);
            control.Reply += PostControl_OnReply;
            control.ImageClick += PostControl_OnImageClick;
            Root.Children.Add(control);
        }

        private void PostControl_OnReply(object sender, PostReplyEventArgs e) {
            Reply(sender, e);
        }

        private void PostControl_OnImageClick(object sender, ImageClickEventArgs e) {
            ImageClick(sender, e);
        }

        private void Post_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            if (RepliesListView.ItemsPanelRoot == null)
                return;

            var total = e.Cumulative.Translation.X;
            var elem =
                RepliesListView.ItemsPanelRoot.Children.OfType<ListViewItem>()
                    .First(lvi => lvi.ContentTemplateRoot == sender);
            var index = RepliesListView.ItemsPanelRoot.Children.IndexOf(elem);

            double itemsCount = RepliesListView.ItemsPanelRoot.Children.Count;

            GoToPostButton.Opacity = GoToThreadButton.Opacity = 1 - Math.Abs(total) / ManipulationAmountToClose;

            for (int i = 0; i < itemsCount; ++i) {
                var distance = Math.Abs(index - i);
                var item = (FrameworkElement)RepliesListView.ItemsPanelRoot.Children[i];
                var translate = item.RenderTransform as TranslateTransform;
                if (translate == null)
                    item.RenderTransform = translate = new TranslateTransform();
                translate.X = total * (1 - distance / itemsCount);
                item.Opacity = Math.Abs(total) > ManipulationAmountToClose ? 0.5 : 1;
            }
        }

        private void Post_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            if (RepliesListView.ItemsPanelRoot == null)
                return;

            if (Math.Abs(e.Cumulative.Translation.X) < ManipulationAmountToClose) {
                GoToPostButton.Opacity = GoToThreadButton.Opacity = 1;
                foreach (var child in RepliesListView.ItemsPanelRoot.Children.Cast<FrameworkElement>()) {
                    var translate = child.RenderTransform as TranslateTransform;
                    if (translate != null)
                        translate.X = 0;
                    child.Opacity = 1;
                    Underlay.Opacity = 1;
                }
            } else {
                Close?.Invoke(this);
            }
        }

        private void Underlay_OnTapped(object sender, TappedRoutedEventArgs e) {
            Close?.Invoke(this);
        }

        private void PostControl_OnReplyShowRequested(object sender, ReplyShowEventArgs e) {
            ReplyShowRequested(sender, e);
        }

        private void PostControl_OnParentPostShowRequested(object sender, ParentPostShowEventArgs e) {

            var control = new PostViewer(SourceThread, e.ThreadNum, e.PostNum);
            control.Close += s => Root.Children.Remove((UIElement)s);
            control.Reply += PostControl_OnReply;
            control.ImageClick += PostControl_OnImageClick;
            control.ReplyShowRequested += PostControl_OnReplyShowRequested;
            Root.Children.Add(control);
        }

        public void GoToPost() {
            if (CanGoToPost)
                Shell.HamburgerMenu
                     .NavigationService
                     .Navigate(typeof (ThreadPage),
                         ThreadNavigation.NavigateToThread(
                             ThreadNum,
                             SourceThread.Board.Id).ToPostByNum(
                                 Posts.First().Num));
        }

        public void GoToThread() {
            if (CanGoToThread)
                Shell.HamburgerMenu
                     .NavigationService
                     .Navigate(typeof (ThreadPage),
                         ThreadNavigation.NavigateToThread(ThreadNum,
                             SourceThread.Board.Id));
        }
    }
}
