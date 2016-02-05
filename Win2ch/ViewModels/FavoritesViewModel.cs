using System.Collections.ObjectModel;
using Win2ch.Models;
using Win2ch.Mvvm;
using Win2ch.Services;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class FavoritesViewModel : ViewModelBase {
        public ObservableCollection<FavoriteThread> FavoriteThreads { get; }
        private FavoritesService FavoritesService { get; } = FavoritesService.Instance;

        public FavoritesViewModel() {
            FavoriteThreads = new ObservableCollection<FavoriteThread>();
            Load();
        }

        private async void Load() {
            var threads = await FavoritesService.GetFavoriteThreads(true);
            foreach (var thread in threads) {
                FavoriteThreads.Add(thread);
            }
        }

        public void GoToThread(FavoriteThread favoriteThread) {
            var thread = new Thread(favoriteThread.Num, favoriteThread.Board.Id);
            NavigationService.Navigate(typeof (ThreadPage), thread);
        }
    }
}
