using Windows.UI.Xaml;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class FavoritesView {
        public FavoritesView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as FavoritesViewModel;
        }

        private FavoritesViewModel ViewModel { get; set; }
    }
}
