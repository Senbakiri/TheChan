using System.Threading.Tasks;
using Windows.Storage;
using Win2ch.Models;

namespace Win2ch.Services {
    public class FavoritesService {
        
        public static FavoritesService Instance { get; } = new FavoritesService();
        
        public FavoriteThreadsStorageService Threads { get; }
        public StorageService<Board> Boards { get; }
        public StorageService<Post> Posts { get; }

        private FavoritesService() {
            var appData = ApplicationData.Current;
            Threads = new FavoriteThreadsStorageService();
            Boards = new StorageService<Board>(
                appData.RoamingFolder, "FavBoards.json");
            Posts = new StorageService<Post>(
                appData.RoamingFolder, "FavPosts.json");
        }
    }
}
