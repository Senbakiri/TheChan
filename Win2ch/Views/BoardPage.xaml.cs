using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Win2ch.Models;
using Win2ch.ViewModels;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Win2ch.Controls;
using Win2ch.Services.SettingsServices;

namespace Win2ch.Views {
    public sealed partial class BoardPage {
        public BoardPage() {
            InitializeComponent();
        }

        public BoardViewModel ViewModel => (BoardViewModel)DataContext;
        public int MaxLinesInPost => SettingsService.Instance.MaxLinesInPostOnBoard;

        private void Thread_OnTapped(object sender, TappedRoutedEventArgs e) {
            if (e.OriginalSource is Image)
                return;

            ViewModel.NavigateToThread((Thread)((FrameworkElement)sender).DataContext);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            if (ImagesViewerUnderlay.Children.Count > 0) {
                ImagesViewerUnderlay.Children.Clear();
                e.Cancel = true;
            }
        }

        private void PostControl_OnImageClick(object sender, ImageClickEventArgs e) {
            var viewer = new ImagesViewer(e.Attachment,
                ViewModel.Threads.SelectMany(t => t.Posts.SelectMany(p => p.Images)).ToList());
            viewer.OnClose += (s, _) => ImagesViewerUnderlay.Children.Clear();
            ImagesViewerUnderlay.Children.Add(viewer);
        }
    }
}
