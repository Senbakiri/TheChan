using Windows.Storage;

namespace TheChan.Services.Storage {
    public class FavoriteThreadsService : ThreadsRepositoryServiceBase {
        public FavoriteThreadsService(IStorageService storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "fvthreads.json") {}
    }
}