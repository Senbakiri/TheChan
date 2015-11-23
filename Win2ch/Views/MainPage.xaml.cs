using System;
using Win2ch.ViewModels;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

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

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dialog = new MessageDialog("...");
            await dialog.ShowAsync();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ViewModel.BoardClick(e.ClickedItem;
        }
    }
}
