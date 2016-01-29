using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Win2ch.Models;

namespace Win2ch.Controls {
    public sealed partial class RepliesListControl {
        public ObservableCollection<Post> Replies { get; }
        public event Action<object> Close;
        public event PostControl.ImageClickEventHandler ImageClick = delegate { };
        public event PostControl.PostReplyEventHandler Reply = delegate { };
        private const int ManipulationAmountToClose = 100;

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
 
            var total = e.Cumulative.Translation.X;
            var elem =
                RepliesListView.ItemsPanelRoot.Children.OfType<ListViewItem>()
                    .First(lvi => lvi.ContentTemplateRoot == sender);
            var index = RepliesListView.ItemsPanelRoot.Children.IndexOf(elem);

            double itemsCount = RepliesListView.ItemsPanelRoot.Children.Count;
            

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
    }
}
