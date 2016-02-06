using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Template10.Controls;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.Mvvm;
using Win2ch.Services;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class FavoritesViewModel : ViewModelBase {
        private bool _IsLoading;
        public ObservableItemCollection<FavoriteThread> FavoriteThreads { get; }
        private FavoritesService FavoritesService { get; } = FavoritesService.Instance;

        public bool IsLoading {
            get { return _IsLoading; }
            set {
                _IsLoading = value;
                RaisePropertyChanged();
            }
        }

        public FavoritesViewModel() {
            FavoriteThreads = new ObservableItemCollection<FavoriteThread>();
            Load();
        }

        private async void Load() {

            try {
                var threads = (await FavoritesService.GetFavoriteThreads()).OrderByDescending(t => t.UnreadPosts);
                FavoriteThreads.Clear();
                foreach (var thread in threads) {
                    FavoriteThreads.Add(thread);
                }
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось загрузить избранные треды");
            }
            
            await Update();
        }

        public async Task Update() {
            IsLoading = true;
            foreach (var thread in FavoriteThreads) {
                try {
                    await FavoritesService.UpdateThread(thread);
                } catch (COMException) {
                    // ничего страшного
                } catch (HttpException) {
                    // всё в порядке
                } catch (ApiException) {
                    await FavoritesService.RemoveThread(thread);
                } catch (Exception e) {
                    await Utils.ShowOtherError(e, "Не удалось загрузить избранные треды");
                }
            }

            IsLoading = false;
        }

        public async Task RemoveThreadFromFavorites(FavoriteThread thread) {
            var succ = await FavoritesService.RemoveThread(thread);
            Load();
        }

        public void GoToThread(FavoriteThread favoriteThread) {
            var thread = new Thread(favoriteThread.Num, favoriteThread.Board.Id);
            NavigationService.Navigate(typeof(ThreadPage), thread);
        }
    }
}
