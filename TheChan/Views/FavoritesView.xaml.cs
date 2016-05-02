using Windows.UI.Xaml.Controls;
using TheChan.ViewModels;

namespace TheChan.Views {
    public sealed partial class FavoritesView {
        public FavoritesView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as FavoritesViewModel;
        }

        private FavoritesViewModel ViewModel { get; set; }

        private void Threads_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.NavigateToThread((ThreadInfoViewModel) e.ClickedItem);
        }
    }
}
