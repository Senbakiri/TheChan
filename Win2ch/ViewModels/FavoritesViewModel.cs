using Core.Common;
using Win2ch.Common.UI;
using Win2ch.Services.Storage;

namespace Win2ch.ViewModels {
    public class FavoritesViewModel : ThreadsRepositoryViewModelBase {
        public FavoritesViewModel(FavoriteThreadsService repositoryService, IBoard board, IShell shell)
            : base(repositoryService, board, shell) {}
    }
}