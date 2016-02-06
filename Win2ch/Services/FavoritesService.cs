using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using Template10.Services.SerializationService;
using Win2ch.Models;

namespace Win2ch.Services {
    public class FavoritesService {
        
        public static FavoritesService Instance { get; } = new FavoritesService();

        private FavoritesService() { }

        private HashSet<FavoriteThread> FavoriteThreads { get; set; } = new HashSet<FavoriteThread>();
        private StorageFolder Folder { get; } = ApplicationData.Current.RoamingFolder;
        private ISerializationService SerializationService { get; } = Template10.Services.SerializationService.SerializationService.Json;

        private const string ThreadsFileName = "FavThreads.json";

        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Пробует добавить тред в избранное
        /// </summary>
        /// <param name="thread">Искомый тред</param>
        /// <returns>Удалось ли добавить</returns>
        public async Task<bool> AddThread(Thread thread) {
            if (!IsLoaded)
                await Load();
            var added = FavoriteThreads.Add(new FavoriteThread(thread));
            if (added) await Store();
            return added;
        }

        private async Task Store() {
            var threadsJson = SerializationService.Serialize(FavoriteThreads);

            await WriteToFile(ThreadsFileName, threadsJson);
        }

        private async Task WriteToFile(string filename, string content) {
            var file = await Folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, content);

        }

        private async Task<string> ReadFromFile(string filename) {
            var file = await Folder.TryGetItemAsync(filename) as IStorageFile;
            if (file == null)
                return null;

            var result = "";
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            using (var reader = new StreamReader(stream.AsStreamForRead()))
                 result = reader.ReadToEnd();
            return result;
        }

        private async Task Load() {
            if (IsLoaded)
                return;

            var threadsJson = await ReadFromFile(ThreadsFileName);
            if (!string.IsNullOrWhiteSpace(threadsJson))
                FavoriteThreads = SerializationService.Deserialize<HashSet<FavoriteThread>>(threadsJson);

            IsLoaded = true;
        }

        /// <summary>
        /// Проверяет, есть ли тред в избранном
        /// </summary>
        /// <param name="thread">Результат проверки</param>
        /// <returns></returns>
        public async Task<bool> IsThreadInFavorites(Thread thread) {
            if (!IsLoaded)
                await Load();
            return FavoriteThreads.Contains(thread);
        }

        /// <summary>
        /// Получить список избранных тредов
        /// </summary>
        public async Task<IReadOnlyCollection<FavoriteThread>> GetFavoriteThreads(bool update = false) {
            if (!IsLoaded)
                await Load();
            if (update)
                await Update();
            return FavoriteThreads.ToList().AsReadOnly();
        }

        public async Task Update() {
            foreach (var thread in FavoriteThreads) {
                var posts = await thread.GetPostsFrom(thread.LastPostPosition + 1);
                thread.UnreadPosts += posts.Count;
                thread.LastPostPosition += posts.Count;
            }
        }

        public async Task ResetThread(Thread thread) {
            var fav = FavoriteThreads.FirstOrDefault(f => Equals(f, thread));
            if (fav == null)
                return;
            fav.UnreadPosts = 0;
            fav.LastPostPosition = thread.Posts.Count;
            await Store();
        }

        public async Task<bool> RemoveThread(Thread thread) {
            if (!IsLoaded)
                await Load();

            var favThread = FavoriteThreads.FirstOrDefault(t => t.Equals(thread));
            var removed = FavoriteThreads.Remove(favThread);
            if (removed) await Store();
            return removed;
        }
    }
}
