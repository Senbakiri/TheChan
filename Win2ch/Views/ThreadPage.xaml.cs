using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Win2ch.Controls;
using Win2ch.Models;
using Win2ch.ViewModels;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Views {
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ThreadPage {
        public ThreadViewModel ViewModel => DataContext as ThreadViewModel;

        private Dictionary<Post, int> _replyLevel { get; } = new Dictionary<Post, int>();
        private Post _lastReply;
        private readonly DispatcherTimer _closeRepliesTimer = new DispatcherTimer();
        private PostControl _replyUnderMouse;
        private readonly bool _isMouseConnected = new MouseCapabilities().MousePresent > 0;

        public ThreadPage() {
            InitializeComponent();
            _closeRepliesTimer.Interval = TimeSpan.FromSeconds(3);
            _closeRepliesTimer.Tick += CloseRepliesTimerOnTick;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            if (e.NavigationMode != NavigationMode.Back)
                return;

            if (Replies.Children.Any()) {
                ClearReplies();
                e.Cancel = true;
            }

            if (RepliesListUnderlay.Children.Any()) {
                RepliesListUnderlay.Children.Clear();
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

            if (e.SelectedText.Length > 0) {
                var pre = FastReplyTextBox.SelectionStart > 0 ? "\n" : "";
                textToAdd = $"{pre}>>{e.Post.Num}\n> {e.SelectedText}\n";
            } else
                textToAdd = $">>{e.Post.Num}\n";

            var selectionIndex = FastReplyTextBox.SelectionStart;
            FastReplyTextBox.Text = FastReplyTextBox.Text.Replace("\r\n", "\n").Insert(selectionIndex, textToAdd);
            FastReplyTextBox.SelectionStart = selectionIndex + textToAdd.Length;
        }

        private void PostControl_OnImageClick(object sender, ImageClickEventArgs e) {
            ViewModel.ShowImageCommand.Execute(e.ImageInfo);
        }

        private void PostControl_OnReplyShowRequested(object sender, ReplyShowEventArgs e) {
            var style = (Style)Resources["ReplyStyle"];
            var position = e.PointerEventArgs.GetCurrentPoint(Replies).Position;
            Post post = e.Post, parent = e.Parent;

            if (post.Equals(_lastReply))
                return;

            var control = new PostControl {
                Post = post,
                Style = style,
            };

            var level = _replyLevel.ContainsKey(parent) ? _replyLevel[parent] + 1 : 1;
            RemoveRepliesByLevel(level);
            _replyLevel[post] = level;
            SetupEventsForReply(control, level);
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

        private void SetupEventsForReply(PostControl control, int level) {
            control.Reply += PostControl_OnReply;
            control.ReplyShowRequested += PostControl_OnReplyShowRequested;
            control.ImageClick += PostControl_OnImageClick;
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
                RemoveRepliesByLevel(_replyLevel[elem.Post]);
            }

        }

        private void PostOnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {

            var total = e.Cumulative.Translation.X;
            var elem = (PostControl) sender;
            var couldBeRemoved = Replies.Children
                .OfType<PostControl>()
                .Where(pc => _replyLevel[pc.Post] >= _replyLevel[elem.Post])
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
                .Where(postControl => _replyLevel[postControl.Post] >= level).ToList();

            foreach (var postControl in controlsToRemove) {
                _replyLevel.Remove(postControl.Post);
                Replies.Children.Remove(postControl);
            }

            _lastReply = null;
        }

        private static double GetControlTopPosition(Point position, UIElement control, PointerRoutedEventArgs eventArgs) {
            var pointerPositionOnControl = eventArgs.GetCurrentPoint(control).Position.Y;
            return position.Y - pointerPositionOnControl;
        }

        private void ClearReplies() {
            _replyLevel.Clear();
            Replies.Children.Clear();
            _lastReply = null;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            ClearReplies();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            Posts.ScrollIntoView(null);
            RepliesListUnderlay.Children.Clear();
        }

        private void PostControl_OnRepliesListShowRequested(Post post) {
            var control = new RepliesListControl(post.Replies);
            control.Close += s => RepliesListUnderlay.Children.Remove((UIElement)s);
            control.ImageClick += PostControl_OnImageClick;
            control.Reply += PostControl_OnReply;
            RepliesListUnderlay.Children.Add(control);
        }

        private void Posts_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            ClearReplies();
        }

        private void Posts_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e) {
            ClearReplies();
        }
    }
}
