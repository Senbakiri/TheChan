using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Win2ch.Models;

namespace Win2ch.Controls {
    public sealed partial class RepliesListControl {
        public ObservableCollection<Post> Replies { get; }

        public event Action<object> Close;

        public event PostControl.ImageClickEventHandler ImageClick = delegate { };
        public event PostControl.PostReplyEventHandler Reply = delegate { };

        public RepliesListControl(IEnumerable<Post> replies) {
            Replies = new ObservableCollection<Post>(replies);
            InitializeComponent();
        }

        private void Underlay_OnTapped(object sender, TappedRoutedEventArgs e) {
            Close?.Invoke(this);
        }

        private void PostControl_OnRepliesListShowRequested(Post post) {
            var control = new RepliesListControl(post.Replies);
            control.Close += s => Grid.Children.Remove((UIElement)s);
            control.Reply += PostControl_OnReply;
            control.ImageClick += PostControl_OnImageClick;
            Grid.Children.Add(control);
        }

        private void PostControl_OnReply(object sender, PostReplyEventArgs e) {
            Reply(sender, e);
        }

        private void PostControl_OnImageClick(object sender, ImageClickEventArgs e) {
            ImageClick(sender, e);
        }
    }
}
