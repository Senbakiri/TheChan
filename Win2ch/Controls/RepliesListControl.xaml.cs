using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private void PostControl_OnRepliesListShowRequested(Post post) {
            var control = new RepliesListControl(post.Replies);
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

            var elem =
                RepliesListView.ItemsPanelRoot.Children.OfType<ListViewItem>()
                    .First(lvi => lvi.ContentTemplateRoot == sender);
            var index = RepliesListView.ItemsPanelRoot.Children.IndexOf(elem);
            var total = e.Cumulative.Translation.X;
            double itemsCount = RepliesListView.ItemsPanelRoot.Children.Count;

            for (int i = 0; i < itemsCount; ++i) {
                var distance = Math.Abs(index - i);
                var item = (FrameworkElement) RepliesListView.ItemsPanelRoot.Children[i];
                var m = item.Margin;
                item.Margin = new Thickness(total * (1 - distance / itemsCount), m.Top, m.Right, m.Bottom);
                item.Opacity = 1 - Math.Abs(total)/100/10*(distance + 1);
                item.MinWidth = item.ActualWidth;
                Underlay.Opacity = 1.0 - total/100/10;
            }
        }

        private void Post_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            if (RepliesListView.ItemsPanelRoot == null)
                return;

            if (e.Cumulative.Translation.X < 250) {
                foreach (var child in RepliesListView.ItemsPanelRoot.Children.Cast<FrameworkElement>()) {
                    child.Margin = new Thickness(0, child.Margin.Top, child.Margin.Right, child.Margin.Bottom);
                    child.Opacity = 1;
                    child.MinWidth = 0;
                    Underlay.Opacity = 1;
                }
            } else {
                Close?.Invoke(this);
            }
        }
    }
}
