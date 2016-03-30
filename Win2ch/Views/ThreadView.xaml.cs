using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Caliburn.Micro;
using Win2ch.Common;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ThreadView : ICanScrollToItem<PostViewModel>, IReplyDisplay {
        private ScrollViewer postsScrollViewer;
        private VirtualizingStackPanel stackPanel;
        private bool dontMarkAsRead;
        private double prevOffset;
        private PostViewModel lastReply, replyUnderMouse;
        private Dictionary<PostViewModel, int> ReplyLevel { get; } = new Dictionary<PostViewModel, int>();
        private readonly bool isMouseConnected = new MouseCapabilities().MousePresent > 0;
        private readonly DispatcherTimer closeRepliesTimer = new DispatcherTimer();

        public ThreadView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ThreadViewModel;
            this.closeRepliesTimer.Interval = TimeSpan.FromSeconds(3);
            this.closeRepliesTimer.Tick += CloseRepliesTimerOnTick;
        }

        public ThreadViewModel ViewModel { get; private set; }

        public void Up() {
            PostViewModel firstPost = ViewModel?.Posts.FirstOrDefault();
            if (firstPost != null)
                this.Posts.ScrollIntoView(firstPost, ScrollIntoViewAlignment.Leading);
        }

        public void Down() {
            PostViewModel lastPost = ViewModel?.Posts.LastOrDefault();
            if (lastPost == null)
                return;

            if (ViewModel.HighlightingStart != 0 && ViewModel.IsHighlighting)
                this.dontMarkAsRead = true;
            this.Posts.ScrollIntoView(lastPost, ScrollIntoViewAlignment.Leading);
        }

        private void ThreadView_OnLoaded(object sender, RoutedEventArgs e) {
            this.postsScrollViewer = GetScrollViewer(this.Posts);
            this.postsScrollViewer.ViewChanged += PostsScrollViewerOnViewChanged;
            this.stackPanel = this.Posts.ItemsPanelRoot as VirtualizingStackPanel;
        }

        private void PostsScrollViewerOnViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
            if (this.stackPanel == null || ViewModel.HighlightingStart == 0 || !ViewModel.IsHighlighting)
                return;

            double offset = this.postsScrollViewer.VerticalOffset;
            double delta = offset - this.prevOffset;
            if (offset >= this.postsScrollViewer.ScrollableHeight) {
                if (!this.dontMarkAsRead)
                    ViewModel.HighlightingStart = ViewModel.Posts.Count + 1;
                if (delta > 0 && delta <= 2)
                    this.dontMarkAsRead = false;
            } else if (!this.dontMarkAsRead || delta > 0) {
                int index = (int)this.stackPanel.VerticalOffset + 1;
                if (index > ViewModel.HighlightingStart)
                    ViewModel.HighlightingStart = index;
                this.dontMarkAsRead = false;
            }

            this.prevOffset = offset;
        }

        private static ScrollViewer GetScrollViewer(ListView listView) {
            var border = (Border)VisualTreeHelper.GetChild(listView, 0);
            return VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
        }

        public void ScrollToItem(PostViewModel item) {
            if (item != null)
                this.Posts.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
        }

        private void CloseRepliesTimerOnTick(object sender, object o) {
            if (this.replyUnderMouse == null)
                ClearReplies();
        }

        private void ClearReplies() {
            ReplyLevel.Clear();
            this.RepliesCanvas.Children.Clear();
            this.lastReply = null;
        }

        public void DisplayReply(ReplyDisplayingEventArgs args) {
            var style = (Style)Resources["PostPopupStyle"];
            Point position = args.PointerEventArgs.GetCurrentPoint(this.RepliesCanvas).Position;
            PostViewModel post = args.Post,
                          parent = args.Parent;

            if (post.Equals(this.lastReply))
                return;

            var view = (FrameworkElement) ViewLocator.LocateForModel(post, null, null);
            view.Style = style;
            ViewModelBinder.Bind(post, view, null);

            int level = ReplyLevel.ContainsKey(parent) ? ReplyLevel[parent] + 1 : 1;
            RemoveRepliesByLevel(level);
            ReplyLevel[post] = level;
            SetupEventsForReply(post, view);
            this.RepliesCanvas.Children.Add(view);

            view.Loaded += (s, _) => {
                HandleGoingBeyondTheWindow(view,
                    new Point(position.X, GetControlTopPosition(position, args.SourceElement, args.PointerEventArgs)),
                    args.SourceElement);
            };

            StartTimer();
            this.lastReply = post;
        }

        private void HandleGoingBeyondTheWindow(FrameworkElement control, Point position, FrameworkElement parent) {
            double x;
            if (ActualWidth <= 480) {
                x = 0;
                control.Width = ActualWidth;
            } else if (position.X < ActualWidth / 2) {
                control.MaxWidth = ActualWidth - position.X;
                x = position.X;
            } else {
                control.MaxWidth = position.X;
                x = position.X - Math.Min(control.ActualWidth, control.MaxWidth);
            }

            Canvas.SetLeft(control, x);

            double y;
            if (position.Y + control.ActualHeight >= this.RepliesCanvas.ActualHeight)
                y = position.Y - control.ActualHeight;
            else
                y = position.Y + parent.ActualHeight;

            Canvas.SetTop(control, y);
        }

        private void SetupEventsForReply(PostViewModel viewModel, UIElement view) {
            ViewModel.SetupEventsForPost(viewModel);
            view.PointerMoved += (s, _) => StartTimer();
            view.ManipulationDelta += PostOnManipulationDelta;
            view.ManipulationCompleted += PostOnManipulationCompleted;
            view.PointerEntered += (s, _) => this.replyUnderMouse = viewModel;
            view.PointerExited += (s, _) => this.replyUnderMouse = this.replyUnderMouse == viewModel ? null : this.replyUnderMouse;
        }

        private void PostOnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            var elem = (PostView)sender;
            if (Math.Abs(e.Cumulative.Translation.X) < 100) {

                foreach (PostView postView in this.RepliesCanvas.Children.OfType<PostView>().ToList()) {
                    postView.Margin = new Thickness(0);
                    postView.Opacity = 1;
                }
            } else {
                RemoveRepliesByLevel(ReplyLevel[elem.ViewModel]);
            }

        }

        private void PostOnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {

            double total = e.Cumulative.Translation.X;
            var elem = (PostView)sender;
            List<PostView> couldBeRemoved = this.RepliesCanvas.Children
                .OfType<PostView>()
                .Where(pc => ReplyLevel[pc.ViewModel] >= ReplyLevel[elem.ViewModel])
                .ToList();
            int count = couldBeRemoved.Count;

            var i = 0.0;
            foreach (PostView postView in couldBeRemoved) {

                postView.Margin = new Thickness(total * (1 - i / count), 0, 0, 0);
                postView.Opacity = 1 - Math.Abs(total) / (200 * (1 - i / count));
                i += 1;
            }
        }

        private void RemoveRepliesByLevel(int level) {
            List<PostView> postsToRemove = this.RepliesCanvas.Children
                .OfType<PostView>()
                .Where(postControl => ReplyLevel[postControl.ViewModel] >= level).ToList();

            foreach (PostView postView in postsToRemove) {
                ReplyLevel.Remove(postView.ViewModel);
                this.RepliesCanvas.Children.Remove(postView);
            }

            this.lastReply = null;
        }

        private static double GetControlTopPosition(Point position, UIElement control, PointerRoutedEventArgs eventArgs) {
            var pointerPositionOnControl = eventArgs.GetCurrentPoint(control).Position.Y;
            return position.Y - pointerPositionOnControl;
        }

        private void StartTimer() {
            if (this.isMouseConnected)
                this.closeRepliesTimer.Start();
        }
    }
}
