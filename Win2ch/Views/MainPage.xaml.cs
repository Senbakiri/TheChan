using Win2ch.Models;
using Win2ch.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Win2ch.Views
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToBoard(e.ClickedItem as Board);
        }
    }
}
