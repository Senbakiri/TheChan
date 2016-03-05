using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Win2ch.Common;
using Win2ch.Controls;
using Win2ch.Models;
using Win2ch.Services.SettingsServices;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ThreadPage : ICanScrollToItem<Attachment>, ICanScrollToItem<Post>, IPositionScroller {
        public ThreadViewModel ViewModel { get; private set; }

        private Dictionary<Post, int> ReplyLevel { get; } = new Dictionary<Post, int>();
        private Post _lastReply;
        private readonly DispatcherTimer _closeRepliesTimer = new DispatcherTimer();
        private PostControl _replyUnderMouse;
        private readonly bool _isMouseConnected = new MouseCapabilities().MousePresent > 0;
        private long _lastRepliedPostNum;
        private ScrollViewer _postsScrollViewer;
        private double _lastVerticalOffsetBeforeScrolling = 0;

        public ThreadPage() {
            InitializeComponent();
            _closeRepliesTimer.Interval = TimeSpan.FromSeconds(3);
            _closeRepliesTimer.Tick += CloseRepliesTimerOnTick;
            DataContextChanged += OnDataContextChanged;
        }

        private void ThreadPage_OnLoaded(object sender, RoutedEventArgs e) {
            ChangeScrollButonView();
            _postsScrollViewer = GetScrollViewer(Posts);
            _postsScrollViewer.ViewChanged += Posts_OnViewChanged;
        }

        private static ScrollViewer GetScrollViewer(ListView listView) {
            var border = (Border)VisualTreeHelper.GetChild(listView, 0);
            return VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            ViewModel = (ThreadViewModel) DataContext;
            ViewModel.PostScroller = this;
            ViewModel.PositionScroller = this;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            if (e.NavigationMode != NavigationMode.Back)
                return;

            if (Replies.Children.Any()) {
                ClearReplies();
                e.Cancel = true;
            }

            if (PostAndRepliesListUnderlay.Children.Any()) {
                PostAndRepliesListUnderlay.Children.Clear();
                e.Cancel = true;
            }

            if (AttachmentViewerUnderlay.Children.Count > 0) {
                AttachmentViewerUnderlay.Children.Clear();
                e.Cancel = true;
            }
        }

        private void StartTimer() {
            if (_isMouseConnected)
                _closeRepliesTimer.Start();
        }

        private void CloseRepliesTimerOnTick(object sender, object o) {
            if (_replyUnderMouse == null)
                ClearReplies();
        }

        private void Header_OnTapped(object sender, TappedRoutedEventArgs e) {
            if (_postsScrollViewer == null)
                return;

            var offset = _lastVerticalOffsetBeforeScrolling;
            _lastVerticalOffsetBeforeScrolling = _postsScrollViewer.VerticalOffset;
            _postsScrollViewer.ChangeView(null, offset, null);
        }

        private void ScrollButton_OnClick(object sender, RoutedEventArgs e) {
            _postsScrollViewer.ChangeView(null,
                _postsScrollViewer.VerticalOffset < _postsScrollViewer.ScrollableHeight
                    ? _postsScrollViewer.ExtentHeight
                    : 0, null);
        }


        private void PostControl_OnReply(object sender, PostReplyEventArgs e) {
            string textToAdd;

            var correctText = FastReplyTextBox.Text.Replace("\r\n", "\n");
            var selStart = FastReplyTextBox.SelectionStart;

            if (e.SelectedText.Length > 0) {
                var pre = selStart > 0 && correctText[selStart - 1] != '\n' ? "\n" : "";
                textToAdd = _lastRepliedPostNum == e.Post.Num && correctText.Contains(_lastRepliedPostNum.ToString())
                    ? $"{pre}\n> {e.SelectedText}\n"
                    : $"{pre}>>{e.Post.Num}\n> {e.SelectedText}\n";
            } else
                textToAdd = $">>{e.Post.Num}\n";

            var selectionIndex = FastReplyTextBox.SelectionStart;
            FastReplyTextBox.Text = correctText.Insert(selectionIndex, textToAdd);
            FastReplyTextBox.SelectionStart = selectionIndex + textToAdd.Length;
            _lastRepliedPostNum = e.Post.Num;
        }

        private void PostControlOnAttachmentClick(object sender, AttachmentClickEventArgs e) {
            var viewer = new AttachmentViewer(
                e.Attachment,
                AttachmentViewerUnderlay,
                ViewModel.Posts.SelectMany(p => p.Attachments)) {
                    Scroller = this
                };

            viewer.Open();
        }

        private void PostControl_OnReplyShowRequested(object sender, ReplyShowEventArgs e) {
            var style = (Style)Resources["PostPopupStyle"];
            var position = e.PointerEventArgs.GetCurrentPoint(Replies).Position;
            Post post = e.Post, parent = e.Parent;

            if (post.Equals(_lastReply))
                return;

            var control = new PostControl {
                Post = post,
                Style = style,
            };

            var level = ReplyLevel.ContainsKey(parent) ? ReplyLevel[parent] + 1 : 1;
            RemoveRepliesByLevel(level);
            ReplyLevel[post] = level;
            SetupEventsForReply(control);
            Replies.Children.Add(control);

            control.Loaded += (s, _) => {
                var senderFrameworkElem = (FrameworkElement)sender;
                HandleGoingBeyondTheWindow(control,
                    new Point(position.X, GetControlTopPosition(position, senderFrameworkElem, e.PointerEventArgs)),
                    senderFrameworkElem);
            };

            StartTimer();
            _lastReply = post;
        }

        private void HandleGoingBeyondTheWindow(FrameworkElement control, Point position, FrameworkElement parent) {
            // if the control could go beyond the window, move it by its size to the opposite side

            double x;
            if (ActualWidth <= 480) {
                x = 0;
                control.Width = ActualWidth;
            } else if (position.X < ActualWidth/2) {
                control.MaxWidth = ActualWidth - position.X;
                x = position.X;
            } else {
                control.MaxWidth = position.X;
                x = position.X - Math.Min(control.ActualWidth, control.MaxWidth);
            }
            
            Canvas.SetLeft(control, x);

            double y;
            if (position.Y + control.ActualHeight >= Replies.ActualHeight)
                y = position.Y - control.ActualHeight;
            else
                y = position.Y + parent.ActualHeight;

            Canvas.SetTop(control, y);
        }

        private void SetupEventsForReply(PostControl control) {
            control.Reply += PostControl_OnReply;
            control.ReplyShowRequested += PostControl_OnReplyShowRequested;
            control.AttachmentClick += PostControlOnAttachmentClick;
            control.ParentPostShowRequested += PostControl_OnParentPostShowRequested;
            control.PointerMoved += (s, _) => StartTimer();
            control.ManipulationDelta += PostOnManipulationDelta;
            control.ManipulationCompleted += PostOnManipulationCompleted;
            control.PointerEntered += (s, _) => _replyUnderMouse = control;
            control.PointerExited += (s, _) => _replyUnderMouse = _replyUnderMouse == control ? null : _replyUnderMouse;
        }

        private void PostOnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            var elem = (PostControl)sender;
            if (Math.Abs(e.Cumulative.Translation.X) < 100) {

                foreach (var control in Replies.Children.OfType<PostControl>().ToList()) {
                    control.Margin = new Thickness(0);
                    control.Opacity = 1;
                }
            }  else {
                RemoveRepliesByLevel(ReplyLevel[elem.Post]);
            }

        }

        private void PostOnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {

            var total = e.Cumulative.Translation.X;
            var elem = (PostControl) sender;
            var couldBeRemoved = Replies.Children
                .OfType<PostControl>()
                .Where(pc => ReplyLevel[pc.Post] >= ReplyLevel[elem.Post])
                .ToList();
            var count = couldBeRemoved.Count;

            var i = 0.0;
            foreach (var control in couldBeRemoved) {

                control.Margin = new Thickness(total * (1 - i / count), 0, 0, 0);
                control.Opacity = 1 - Math.Abs(total) / (200 * (1 - i / count));

                i += 1;
            }
        }

        private void RemoveRepliesByLevel(int level) {
            var controlsToRemove = Replies.Children
                .OfType<PostControl>()
                .Where(postControl => ReplyLevel[postControl.Post] >= level).ToList();

            foreach (var postControl in controlsToRemove) {
                ReplyLevel.Remove(postControl.Post);
                Replies.Children.Remove(postControl);
            }

            _lastReply = null;
        }

        private static double GetControlTopPosition(Point position, UIElement control, PointerRoutedEventArgs eventArgs) {
            var pointerPositionOnControl = eventArgs.GetCurrentPoint(control).Position.Y;
            return position.Y - pointerPositionOnControl;
        }

        private void ClearReplies() {
            ReplyLevel.Clear();
            Replies.Children.Clear();
            _lastReply = null;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            ClearReplies();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            _lastVerticalOffsetBeforeScrolling = 0;
            ChangeScrollButonView();
            if (e.NavigationMode == NavigationMode.Back)
                return;

            PostAndRepliesListUnderlay.Children.Clear();
        }

        private void PostControl_OnRepliesListShowRequested(Post post) {
            var control = new PostViewer(ViewModel.Thread, post.Replies);
            control.Close += s => PostAndRepliesListUnderlay.Children.Remove((UIElement)s);
            control.ImageClick += PostControlOnAttachmentClick;
            control.Reply += PostControl_OnReply;
            PostAndRepliesListUnderlay.Children.Add(control);
        }

        private void Posts_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            ClearReplies();
        }

        private void Posts_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e) {
            ClearReplies();
        }

        private async void FastReply() {
            var isSuccess = await ViewModel.FastReply();
            if (isSuccess)
                Posts.ScrollIntoView(Posts.Items.Last(), ScrollIntoViewAlignment.Leading);
        }

        public async void Refresh() {
            await ViewModel.Refresh();
        }

        private void PostControl_OnParentPostShowRequested(object sender, ParentPostShowEventArgs e) {
            ClearReplies();
            var control = new PostViewer(ViewModel.Thread, e.ThreadNum, e.PostNum);
            control.Close += s => PostAndRepliesListUnderlay.Children.Remove((UIElement)s);
            control.ImageClick += PostControlOnAttachmentClick;
            control.Reply += PostControl_OnReply;
            control.ReplyShowRequested += PostControl_OnReplyShowRequested;
            PostAndRepliesListUnderlay.Children.Add(control);
        }

        public void ScrollToItem(Post item) {
            Posts.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
        }

        private void ThreadPage_OnKeyDown(object sender, KeyRoutedEventArgs e) {
            switch (e.Key) {
                case VirtualKey.Home:
                    Posts.ScrollIntoView(null);
                    break;
                case VirtualKey.End:
                    if (Posts.Items != null)
                        Posts.ScrollIntoView(Posts.Items.Last(), ScrollIntoViewAlignment.Leading);
                    break;
            }
        }

        private void CommandBar_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
                e.Complete();
        }

        private void CommandBar_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            var total = e.Cumulative.Translation.X;
            if (total < -75)
                ViewModel.GoToBoard();
        }

        public double Position {
            get { return _postsScrollViewer.VerticalOffset; }
            set { _postsScrollViewer.ChangeView(null, value, null); }
        }

        private void Posts_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
            ChangeScrollButonView();
        }

        private void ChangeScrollButonView() {
            if (_postsScrollViewer == null)
                VisualStateManager.GoToState(this, "Up", true);
            else
                VisualStateManager.GoToState(this,
                    _postsScrollViewer.VerticalOffset >= _postsScrollViewer.ScrollableHeight ? "Down" : "Up",
                    true);
        }

        public void ScrollToItem(Attachment item) {
            var post = ViewModel.Posts.FirstOrDefault(p => p.Attachments?.Contains(item) ?? false);
            if (post != null)
                Posts.ScrollIntoView(post, ScrollIntoViewAlignment.Leading);
        }
    }
}
