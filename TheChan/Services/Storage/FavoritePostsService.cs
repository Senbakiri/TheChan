using Windows.Storage;
using Core.Models;

namespace TheChan.Services.Storage {
    public class FavoritePostsService : ItemsRepositoryServiceBase<Post> {
        public FavoritePostsService(IStorageService storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "fvposts.json") {}
    }
}