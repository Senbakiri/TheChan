using System.Collections.Generic;
using Windows.Storage;
using Core.Models;

namespace Win2ch.Services.Storage {
    public class FavoritePostsService : ItemsRepositoryServiceBase<Post> {
        public FavoritePostsService(IStorageService<IList<Post>> storageService)
            : base(storageService, ApplicationData.Current.RoamingFolder, "favposts.json") {}
    }
}