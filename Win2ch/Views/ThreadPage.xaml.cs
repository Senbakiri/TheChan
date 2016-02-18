﻿using System;
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
    public sealed partial class ThreadPage : ICanScrollToItem<Post> {
        public ThreadViewModel ViewModel { get; private set; }

        private Dictionary<Post, int> ReplyLevel { get; } = new Dictionary<Post, int>();
        private Post _lastReply;
        private readonly DispatcherTimer _closeRepliesTimer = new DispatcherTimer();
        private PostControl _replyUnderMouse;
        private readonly bool _isMouseConnected = new MouseCapabilities().MousePresent > 0;
        private long _lastRepliedPostNum;

        public ThreadPage() {
            InitializeComponent();
            _closeRepliesTimer.Interval = TimeSpan.FromSeconds(3);
            _closeRepliesTimer.Tick += CloseRepliesTimerOnTick;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            ViewModel = (ThreadViewModel) DataContext;
            ViewModel.PostScroller = this;
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

            if (ImagesViewerUnderlay.Children.Count > 0) {
                ImagesViewerUnderlay.Children.Clear();
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
            if (Posts.Items != null && Posts.Items.Count > 0)
                Posts.ScrollIntoView(Posts.Items[0]);
        }

        private void ScrollDown_OnClicked(object sender, RoutedEventArgs e) {
            if (Posts.Items != null && Posts.Items.Count > 0)
                Posts.ScrollIntoView(Posts.Items[Posts.Items.Count - 1]);
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

        private void PostControl_OnImageClick(object sender, ImageClickEventArgs e) {
            var viewer = new ImagesViewer(e.ImageInfo,
                ViewModel.Posts.SelectMany(p => p.Images).ToList());

            viewer.OnClose += ImagesViewerOnClose;
            ImagesViewerUnderlay.Children.Add(viewer);
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
            // if the control could go beyond the window, move it by his size to the opposite side

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
            control.ImageClick += PostControl_OnImageClick;
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
            if (e.NavigationMode == NavigationMode.Back)
                return;

            PostAndRepliesListUnderlay.Children.Clear();
        }

        private void PostControl_OnRepliesListShowRequested(Post post) {
            var control = new PostViewer(ViewModel.Thread, post.Replies);
            control.Close += s => PostAndRepliesListUnderlay.Children.Remove((UIElement)s);
            control.ImageClick += PostControl_OnImageClick;
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
            var hasNewPosts = await ViewModel.Refresh();
            if (hasNewPosts)
                Posts.ScrollIntoView(Posts.Items.Last(), ScrollIntoViewAlignment.Leading);
        }

        private void ImagesViewerOnClose(object sender, ImagesViewerCloseEventArgs e) {
            ImagesViewerUnderlay.Children.Clear();

            if (!SettingsService.Instance.ScrollToPostWithImageAfterViewingImage)
                return;

            var post = ViewModel.Posts.FirstOrDefault(p => p.Images.Contains(e.LastImage));
            if (post != null)
                Posts.ScrollIntoView(post, ScrollIntoViewAlignment.Leading);
        }

        private void PostControl_OnParentPostShowRequested(object sender, ParentPostShowEventArgs e) {
            ClearReplies();
            var control = new PostViewer(ViewModel.Thread, e.ThreadNum, e.PostNum);
            control.Close += s => PostAndRepliesListUnderlay.Children.Remove((UIElement)s);
            control.ImageClick += PostControl_OnImageClick;
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
        
    }
}
