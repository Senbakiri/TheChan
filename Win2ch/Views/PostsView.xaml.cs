using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class PostsView {
        private const double ManipulationAmountToClose = 100;

        public PostsView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as PostsViewModel;
        }

        public PostsViewModel ViewModel { get; private set; }

        private void Underlay_OnTapped(object sender, TappedRoutedEventArgs e) {
            ViewModel.CloseDown();
        }

        private void Post_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            if (this.List.ItemsPanelRoot == null)
                return;

            double total = e.Cumulative.Translation.X;
            ListViewItem elem =
                this.List.ItemsPanelRoot.Children.OfType<ListViewItem>()
                    .First(lvi => lvi.ContentTemplateRoot == sender);
            int index = this.List.ItemsPanelRoot.Children.IndexOf(elem);

            double itemsCount = this.List.ItemsPanelRoot.Children.Count;

            this.GoToPostButton.Opacity = this.GoToThreadButton.Opacity = 1 - Math.Abs(total) / ManipulationAmountToClose;

            for (var i = 0; i < itemsCount; ++i) {
                int distance = Math.Abs(index - i);
                var item = (FrameworkElement)this.List.ItemsPanelRoot.Children[i];
                var translate = item.RenderTransform as TranslateTransform;
                if (translate == null)
                    item.RenderTransform = translate = new TranslateTransform();
                translate.X = total * (1 - distance / itemsCount);
                item.Opacity = Math.Abs(total) > ManipulationAmountToClose ? 0.5 : 1;
            }
        }

        private void Post_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            if (this.List.ItemsPanelRoot == null)
                return;

            if (Math.Abs(e.Cumulative.Translation.X) < ManipulationAmountToClose) {
                this.GoToPostButton.Opacity = this.GoToThreadButton.Opacity = 1;
                foreach (FrameworkElement child in this.List.ItemsPanelRoot.Children.Cast<FrameworkElement>()) {
                    var translate = child.RenderTransform as TranslateTransform;
                    if (translate != null)
                        translate.X = 0;
                    child.Opacity = 1;
                }
            } else {
                ViewModel.CloseDown();
            }
        }
    }
}
