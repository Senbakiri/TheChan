using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Win2ch.ViewModels;
using WinRTXamlToolkit.Controls.Extensions;

namespace Win2ch.Views {
    public sealed partial class ThreadView {
        private ScrollViewer postsScrollViewer;

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
            if (lastPost != null)
                this.Posts.ScrollIntoView(lastPost, ScrollIntoViewAlignment.Leading);
        }

        private void ThreadView_OnLoaded(object sender, RoutedEventArgs e) {
            this.postsScrollViewer = GetScrollViewer(this.Posts);
            this.postsScrollViewer.ViewChanged += PostsScrollViewerOnViewChanged;
        }

        private void PostsScrollViewerOnViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
            double offset = this.postsScrollViewer.VerticalOffset;
            if (offset >= this.postsScrollViewer.ScrollableHeight) {
                ViewModel.HighlightingStart = ViewModel.Posts.Count + 1;
            } else {
                int index = this.Posts.GetFirstVisibleIndex() + 1;
                if (index > ViewModel.HighlightingStart)
                    ViewModel.HighlightingStart = index;
            }
        }

        private static ScrollViewer GetScrollViewer(ListView listView) {
            var border = (Border)VisualTreeHelper.GetChild(listView, 0);
            return VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
        }
    }
}
