using System;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Win2ch.Models;
using Win2ch.ViewModels;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Win2ch.Controls;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Views {
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BoardPage {
        public BoardPage() {
            InitializeComponent();
        }

        public BoardViewModel ViewModel => (BoardViewModel)DataContext;

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
            var viewer = new ImagesViewer(e.ImageInfo,
                ViewModel.Threads.SelectMany(t => t.Posts.SelectMany(p => p.Images)).ToList());
            viewer.OnClose += (s, _) => ImagesViewerUnderlay.Children.Clear();
            ImagesViewerUnderlay.Children.Add(viewer);
        }
    }
}
