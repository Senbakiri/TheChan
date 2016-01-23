using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
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

namespace Win2ch.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ThreadPage
    {
        public ThreadViewModel ViewModel => DataContext as ThreadViewModel;

        private Dictionary<Post, int> _replyLevel { get; } = new Dictionary<Post, int>();
        private Post _lastReply;

        public ThreadPage()
        {
            InitializeComponent();
        }

        private void Header_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (Posts.Items != null && Posts.Items.Count > 0)
                Posts.ScrollIntoView(Posts.Items[0]);
        }

        private void ScrollDown_OnClicked(object sender, RoutedEventArgs e)
        {
            if (Posts.Items != null && Posts.Items.Count > 0)
                Posts.ScrollIntoView(Posts.Items[Posts.Items.Count - 1]);
        }
        

        private void PostControl_OnReply(object sender, PostReplyEventArgs e)
        {
            string textToAdd;

            if (e.SelectedText.Length > 0)
            {
                var pre = FastReplyTextBox.SelectionStart > 0 ? "\n" : "";
                textToAdd = $"{pre}>>{e.Post.Num}\n> {e.SelectedText}\n";
            }
            else
                textToAdd = $">>{e.Post.Num}\n";

            var selectionIndex = FastReplyTextBox.SelectionStart;
            FastReplyTextBox.Text = FastReplyTextBox.Text.Insert(selectionIndex, textToAdd);
            FastReplyTextBox.SelectionStart = selectionIndex + textToAdd.Length;
        }

        private void PostControl_OnImageClick(object sender, ImageClickEventArgs e)
        {
            ViewModel.ShowImageCommand.Execute(e.ImageInfo);
        }

        private void PostControl_OnReplyShowRequested(object sender, ReplyShowEventArgs e)
        {

            var style = (Style) Resources["ReplyStyle"];
            var position = e.PointerEventArgs.GetCurrentPoint(Replies).Position;
            var post = e.Post;
            var parent = e.Parent;
            var width = Replies.ActualWidth;

            if (post.Equals(_lastReply))
                return;

            var control = new PostControl
            {
                Post = post,
                Style = style,
                MaxWidth = width - position.X
            };
            
            var level = _replyLevel.ContainsKey(parent) ? _replyLevel[parent] + 1 : 1;

            var controlsToRemove = new List<PostControl>();

            // remove all existing controls that on current level or lower
            foreach (var postControl in Replies.Children.OfType<PostControl>())
            {
                if (_replyLevel[postControl.Post] >= level)
                    controlsToRemove.Add(postControl);
            }

            foreach (var postControl in controlsToRemove)
            {
                _replyLevel.Remove(postControl.Post);
                Replies.Children.Remove(postControl);
            }

            control.ReplyShowRequested += PostControl_OnReplyShowRequested;

            _replyLevel[post] = level;

            Replies.Children.Add(control);

            Canvas.SetLeft(control, position.X);
            Canvas.SetTop(control, GetControlBottomYPosition(position, (FrameworkElement) sender, e.PointerEventArgs));

            _lastReply = post;
        }

        private double GetControlBottomYPosition(Point position, FrameworkElement control, PointerRoutedEventArgs eventArgs)
        {
            var pointerPositionOnControl = eventArgs.GetCurrentPoint(control).Position.Y;
            return position.Y - pointerPositionOnControl + control.ActualHeight;
        }

        private void ClearReplies()
        {
            _replyLevel.Clear();
            Replies.Children.Clear();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ClearReplies();
        }
    }
}
