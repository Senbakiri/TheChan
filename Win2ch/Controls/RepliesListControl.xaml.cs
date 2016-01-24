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
            Grid.Children.Add(control);
        }
    }
}
