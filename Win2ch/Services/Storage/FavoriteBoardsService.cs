using System.Collections.Generic;
using Windows.Storage;
using Core.Models;

namespace Win2ch.Services.Storage {
    public class FavoriteBoardsService : ItemsRepositoryServiceBase<BriefBoardInfo> {
        public FavoriteBoardsService(IStorageService storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "fvboards.json") {}
    }
}