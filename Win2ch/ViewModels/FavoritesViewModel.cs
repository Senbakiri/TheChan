using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Template10.Controls;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.Mvvm;
using Win2ch.Services;
using Win2ch.Views;

namespace Win2ch.ViewModels {
    public class FavoritesViewModel : ViewModelBase {
        private bool _IsLoading;
        private FavoritesService FavoritesService { get; } = FavoritesService.Instance;

        public ObservableItemCollection<StoredThreadInfo> FavoriteThreads { get; }
        public ObservableCollection<Post> FavoritePosts { get; }

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
            FavoriteThreads = new ObservableItemCollection<StoredThreadInfo>();
            FavoritePosts = new ObservableCollection<Post>();
        }

        public async Task Load() {
            if (IsLoading)
                return;
            IsLoading = true;

            try {
                var threads = (await FavoritesService.Threads.GetItems()).OrderByDescending(t => t.UnreadPosts);
                Fill(threads, FavoriteThreads);

                var posts = await FavoritesService.Posts.GetItems();
                Fill(posts, FavoritePosts);
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось загрузить избранные треды");
            }
            
            await Update();
        }

        private static void Fill<T>(IEnumerable<T> source, IList<T> target) {
            target.Clear();
            foreach (var post in source) {
                target.Add(post);
            }
        }

        public async Task Update() {
            IsLoading = true;
            foreach (var thread in FavoriteThreads) {
                try {
                    await FavoritesService.Threads.UpdateThread(thread);
                } catch (COMException) {
                    // ничего страшного
                } catch (HttpException) {
                    // всё в порядке
                } catch (ApiException) {
                    await FavoritesService.Threads.RemoveThread(thread);
                } catch (Exception e) {
                    await Utils.ShowOtherError(e, "Не удалось загрузить избранные треды");
                }
            }

            try {
                await FavoritesService.Threads.Store();
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Сохранение не удалось");
            }

            IsLoading = false;
        }

        public async Task RemoveThreadFromFavorites(StoredThreadInfo thread) {
            try {
                var removed = await FavoritesService.Threads.RemoveThread(thread);
                if (removed)
                    FavoriteThreads.Remove(thread);
            } catch (Exception e) {
                await Utils.ShowOtherError(e, "Не удалось удалить тред");
            }

            await Update();
        }

        public void GoToThread(StoredThreadInfo storedThreadInfo) {
            NavigationService.Navigate(typeof (ThreadPage),
                new ThreadNavigationInfo(storedThreadInfo.Num,
                    storedThreadInfo.Board.Id,
                    storedThreadInfo.LastPostPosition - storedThreadInfo.UnreadPosts + 1,
                    true) {
                        ForceRefresh = true
                    });
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) {
            await Load();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
