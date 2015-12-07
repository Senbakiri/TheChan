using System.Collections.ObjectModel;
using Win2ch.Models;
using Win2ch.Views;

namespace Win2ch.ViewModels
{
    public class MainPageViewModel : Mvvm.ViewModelBase
    {

        public MainPageViewModel()
        {
            Categories = new ObservableCollection<Category>();

            LoadBoards();
        }

        public ObservableCollection<Category> Categories
        { get; set; }

        private async void LoadBoards()
        {
            var categories = await new BoardsProvider().GetCategories();
            foreach (var cat in categories)
                Categories.Add(cat);
        }

        public void NavigateToBoard(Board board)
        {
            NavigationService.Navigate(typeof(BoardPage), board);
        }
    }
}

