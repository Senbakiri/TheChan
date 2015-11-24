using System;
using Win2ch.Models;
using Win2ch.ViewModels;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Win2ch.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public MainPageViewModel ViewModel => this.DataContext as MainPageViewModel;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToBoard(e.ClickedItem as Board);
        }
    }
}
