using Windows.UI.Xaml.Controls;
using Core.Models;
using Win2ch.ViewModels;

namespace Win2ch.Views {
    public sealed partial class HomeView {
        public HomeView() {
            InitializeComponent();
            DataContextChanged += (s, e) => ViewModel = DataContext as HomeViewModel;
        }

        private HomeViewModel ViewModel { get; set; }

        private void BoardsListView_OnItemClick(object sender, ItemClickEventArgs e) {
            ViewModel.NavigateToBoard(e.ClickedItem as BriefBoardInfo);
        }

        private void FastNavigationTextBox_OnTextChanged(object sender, TextChangedEventArgs e) {
            this.CategoriesCvs.Source = ViewModel.FilterItems(this.FastNavigation.Text);
        }
    }
}
