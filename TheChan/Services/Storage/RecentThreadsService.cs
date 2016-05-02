using Windows.Storage;

namespace TheChan.Services.Storage {
    public class RecentThreadsService : ThreadsRepositoryServiceBase {
        public RecentThreadsService(IStorageService storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "recthreads.json") { }
    }
}