using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Win2ch.Models;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class RecentThreadsPage {
        public RecentThreadsViewModel ViewModel { get; private set; }

        public RecentThreadsPage() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = (RecentThreadsViewModel) DataContext;
        }

        private void RecentThreads_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.GoToThread((StoredThreadInfo) e.ClickedItem);
        }

        public void ResizeThreads() {
            if ((int)Root.ActualWidth == 0 || Root.ActualWidth > 500)
                return;

            if (RecentThreadsGrid.ItemsPanelRoot == null)
                return;

            foreach (var item in RecentThreadsGrid.ItemsPanelRoot.Children.Cast<GridViewItem>()) {
                var child = item.ContentTemplateRoot as FrameworkElement;
                if (child != null)
                    child.Width = Math.Floor(Root.ActualWidth - RecentThreadsGrid.Padding.Left - RecentThreadsGrid.Padding.Right) / 2
                        - (item.Margin.Left + item.Margin.Right)
                        - (child.Margin.Left + child.Margin.Right)
                        - 1;
            }
        }

        private async void RemoveThreadFromRecentMenuFlyoutItem_OnClick(object sender, RoutedEventArgs e) {
            await ViewModel.RemoveThreadFromRecent((StoredThreadInfo)((FrameworkElement)sender).DataContext);
        }
    }
}
