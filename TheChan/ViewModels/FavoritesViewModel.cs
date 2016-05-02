using System.Collections.ObjectModel;
using System.Linq;
using Core.Common;
using Core.Models;
using TheChan.Common.UI;
using TheChan.Extensions;
using TheChan.Services.Storage;

namespace TheChan.ViewModels {
    public class FavoritesViewModel : ThreadsRepositoryViewModelBase {
        public FavoritesViewModel(FavoriteThreadsService threadsRepositoryService,
                                  FavoritePostsService favoritePostsService,
                                  IBoard board,
                                  IShell shell)
            : base(threadsRepositoryService, board, shell) {
            FavoritePosts = favoritePostsService;
            Posts = new ObservableCollection<PostViewModel>();
        }

        private FavoritePostsService FavoritePosts { get; }

        public ObservableCollection<PostViewModel> Posts { get; }

        public override void Load() {
            base.Load();
            Posts.Clear();
            Posts.AddRange(FavoritePosts.Items.Select(CreatePostViewModel));
        }

        private PostViewModel CreatePostViewModel(Post post) {
            return new PostViewModel {
                Post = post,
                IsTextSelectionEnabled = true,
                ShowPostPosition = false,
                ShowReplies = false
            };
        }

        public async void Unfavorite(PostViewModel post) {
            post.IsInFavorites = false;
            FavoritePosts.Items.Remove(post.Post);
            Posts.Remove(post);
            await FavoritePosts.Save();
        }
    }
}