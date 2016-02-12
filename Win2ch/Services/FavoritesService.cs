using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Win2ch.Models;

namespace Win2ch.Services {
    public class FavoritesService {
        
        public static FavoritesService Instance { get; } = new FavoritesService();
        
        private StorageService<StoredThreadInfo> FavoriteThreadsStorageService { get; }

        private FavoritesService() {
            var appData = ApplicationData.Current;
            FavoriteThreadsStorageService = new StorageService<StoredThreadInfo>(
                appData.RoamingFolder, "FavThreads.json");
        }

        /// <summary>
        /// Пробует добавить тред в избранное
        /// </summary>
        /// <param name="thread">Искомый тред</param>
        /// <returns>Удалось ли добавить</returns>
        public async Task<bool> AddThread(Thread thread) {
            return await FavoriteThreadsStorageService.Add(new StoredThreadInfo(thread));
        }

        /// <summary>
        /// Проверяет, есть ли тред в избранном
        /// </summary>
        /// <param name="thread">Результат проверки</param>
        /// <returns></returns>
        public async Task<bool> IsThreadInFavorites(Thread thread) {
            return await FavoriteThreadsStorageService.ContainsItem(new StoredThreadInfo(thread));
        }

        /// <summary>
        /// Получить список избранных тредов
        /// </summary>
        public async Task<IReadOnlyCollection<StoredThreadInfo>> GetFavoriteThreads() {
            return await FavoriteThreadsStorageService.GetItems();
        }

        public async Task<bool> UpdateThread(Thread thread) {
            var threads = await FavoriteThreadsStorageService.GetItems();
            var fav = threads.FirstOrDefault(f => Equals(f, thread));
            if (fav == null)
                return false;
            var posts = await thread.GetPostsFrom(fav.LastPostPosition + 1);
            fav.UnreadPosts += posts.Count;
            fav.LastPostPosition += posts.Count;
            return true;
        }

        public async Task ResetThread(Thread thread) {
            var threads = await FavoriteThreadsStorageService.GetItems();
            var fav = threads.FirstOrDefault(f => Equals(f, thread));
            if (fav == null)
                return;
            fav.UnreadPosts = 0;
            fav.LastPostPosition = thread.Posts.Count;
            await FavoriteThreadsStorageService.Store();
        }

        public async Task<bool> RemoveThread(Thread thread) {
            var threads = await FavoriteThreadsStorageService.GetItems();

            var favoriteThread = thread as StoredThreadInfo;
            var favThread = favoriteThread ?? threads.FirstOrDefault(t => t.Equals(thread));
            var removed = await FavoriteThreadsStorageService.RemoveItem(favThread);
            return removed;
        }
    }
}
