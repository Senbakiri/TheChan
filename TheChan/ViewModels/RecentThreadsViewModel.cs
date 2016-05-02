using Core.Common;
using TheChan.Common.UI;
using TheChan.Services.Storage;

namespace TheChan.ViewModels {
    public class RecentThreadsViewModel : ThreadsRepositoryViewModelBase {
        public RecentThreadsViewModel(RecentThreadsService repositoryService, IBoard board, IShell shell)
            : base(repositoryService, board, shell) {}
    }
}