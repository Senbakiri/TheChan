using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Win2ch.Models;

namespace Win2ch.Services {
    public class FavoritesService {
        
        public static FavoritesService Instance { get; } = new FavoritesService();
        
        public ThreadStorageService Threads { get; }

        private FavoritesService() {
            var appData = ApplicationData.Current;
            Threads = new ThreadStorageService(
                appData.RoamingFolder, "FavThreads.json");
        }
    }
}
