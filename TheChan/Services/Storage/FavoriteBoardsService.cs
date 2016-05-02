using Windows.Storage;
using Core.Models;

namespace TheChan.Services.Storage {
    public class FavoriteBoardsService : ItemsRepositoryServiceBase<BriefBoardInfo> {
        public FavoriteBoardsService(IStorageService storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "fvboards.json") {}
    }
}