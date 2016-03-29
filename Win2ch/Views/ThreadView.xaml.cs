using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Win2ch.Common;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class ThreadView : ICanScrollToItem<PostViewModel> {
        private ScrollViewer postsScrollViewer;
        private VirtualizingStackPanel stackPanel;
        private bool dontMarkAsRead;
        private double prevOffset;

        public ThreadView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as ThreadViewModel;
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
    }
}
