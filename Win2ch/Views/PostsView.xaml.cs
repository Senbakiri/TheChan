using Windows.UI.Xaml.Input;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class PostsView {
        public PostsView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as PostsViewModel;
        }

        public PostsViewModel ViewModel { get; private set; }

        private void Underlay_OnTapped(object sender, TappedRoutedEventArgs e) {
            ViewModel.CloseDown();
        }
    }
}
