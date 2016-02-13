using Windows.Storage;
using Win2ch.Models;

namespace Win2ch.Services {
    public class FavoritesService {
        
        public static FavoritesService Instance { get; } = new FavoritesService();
        
        public ThreadStorageService Threads { get; }
        public StorageService<Board> Boards { get; } 

        private FavoritesService() {
            var appData = ApplicationData.Current;
            Threads = new ThreadStorageService(
                appData.RoamingFolder, "FavThreads.json");
            Boards = new StorageService<Board>(
                appData.RoamingFolder, "FavBoards.json");
        }
    }
}
