using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Win2ch.Models;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Data;

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

    }
}

