using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Template10.Controls;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.Mvvm;
using Win2ch.Services;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class FavoritesViewModel : ViewModelBase {
        private bool _IsLoading;
        public ObservableItemCollection<StoredThreadInfo> FavoriteThreads { get; }
        private FavoritesService FavoritesService { get; } = FavoritesService.Instance;

        public bool IsLoading {
            get { return _IsLoading; }
            set {
                if (value == _IsLoading)
                    return;
                _IsLoading = value;
                RaisePropertyChanged();
            }
        }

        public FavoritesViewModel() {
            FavoritesService.Updated += FavoritesServiceOnUpdated;
            FavoriteThreads = new ObservableItemCollection<StoredThreadInfo>();
            Load();
        }

        private void FavoritesServiceOnUpdated(FavoritesService s) {
            Window.Current.Activated += (_, e) => {
                if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
                    Load();
            };
        }

        public async void Load() {
            if (IsLoading)
                return;
            IsLoading = true;

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

        public async Task RemoveThreadFromFavorites(StoredThreadInfo thread) {
            await FavoritesService.RemoveThread(thread);
            Load();
        }

        public void GoToThread(StoredThreadInfo storedThreadInfo) {
            var thread = new Thread(storedThreadInfo.Num, storedThreadInfo.Board.Id);
            NavigationService.Navigate(typeof(ThreadPage), thread);
        }
    }
}
