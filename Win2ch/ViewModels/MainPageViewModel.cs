using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Win2ch.Models;
using Win2ch.Models.Api;
using Win2ch.Views;

namespace Win2ch.ViewModels
{
    public class MainPageViewModel : Mvvm.ViewModelBase
    {

        public MainPageViewModel()
        {
            Categories = new ObservableCollection<Category>();
        }

        public ObservableCollection<Category> Categories
        { get; set; }

        private List<Category> HiddenCategories { get; } = new List<Category>(); 

        public async void LoadBoards()
        {
            var categories = await new ApiBoardsProvider().GetCategories();
            foreach (var cat in categories)
            {
                if (cat.Name != "��������")
                    Categories.Add(cat);
                else
                    HiddenCategories.Add(cat);
            }
        }

        public void NavigateToBoard(Board board)
        {
            NavigationService.Navigate(typeof(BoardPage), board);
        }
    }
}

