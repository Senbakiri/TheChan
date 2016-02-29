using System.Threading.Tasks;
using Windows.Storage;

namespace Win2ch.Services {
    public class FavoriteThreadsStorageService : ThreadStorageService {
        public FavoriteThreadsStorageService() : base(ApplicationData.Current.RoamingFolder, "FavThreads.json") { }

        public override async Task Store() {
            await base.Store();
            await LiveTileService.Instance.Update();
            await BadgeService.Instance.Update();
        }
    }
}