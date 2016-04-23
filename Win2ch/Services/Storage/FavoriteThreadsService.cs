using System.Collections.Generic;
using Windows.Storage;
using Core.Models;

namespace Win2ch.Services.Storage {
    public class FavoriteThreadsService : ThreadsRepositoryServiceBase {
        public FavoriteThreadsService(IStorageService<IList<ThreadInfo>> storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "fvthreads.json") {}
    }
}