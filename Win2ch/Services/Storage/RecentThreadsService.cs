﻿using System.Collections.Generic;
using Windows.Storage;
using Core.Models;

namespace Win2ch.Services.Storage {
    public class RecentThreadsService : ThreadsRepositoryServiceBase {
        public RecentThreadsService(IStorageService<ICollection<ThreadInfo>> storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "recthreads.json") { }
    }
}