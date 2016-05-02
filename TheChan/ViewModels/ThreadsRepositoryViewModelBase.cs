using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common;
using Core.Models;
using TheChan.Common;
using TheChan.Common.UI;
using TheChan.Extensions;
using TheChan.Services.Storage;

namespace TheChan.ViewModels {
    public abstract class ThreadsRepositoryViewModelBase : PropertyChangedBase {

        protected ThreadsRepositoryViewModelBase(IThreadsRepositoryService repositoryService, IBoard board, IShell shell) {
            RepositoryService = repositoryService;
            Board = board;
            Shell = shell;
            Threads = new ObservableCollection<ThreadInfoViewModel>();
        }

        private IBoard Board { get; }
        private IShell Shell { get; }
        private IThreadsRepositoryService RepositoryService { get; }
        private bool IsLoading { get; set; }
        public ObservableCollection<ThreadInfoViewModel> Threads { get; }

        public virtual async void Load() {
            if (IsLoading)
                return;

            IsLoading = true;
            Threads.Clear();
            Threads.AddRange(RepositoryService.Items.Select(t => new ThreadInfoViewModel(t)));
            foreach (ThreadInfoViewModel thread in Threads) {
                await UpdateThread(thread);
            }

            await RepositoryService.Save();
            IsLoading = false;
        }

        public async void RefreshAll() {
            if (IsLoading)
                return;

            IsLoading = true;
            foreach (ThreadInfoViewModel thread in Threads) {
                await UpdateThread(thread);
            }

            await RepositoryService.Save();
            IsLoading = false;
        }

        private async Task UpdateThread(ThreadInfoViewModel threadInfo) {
            threadInfo.IsLoading = true;
            try {
                Thread newThread = await Board.LoadThreadAsync(threadInfo.ThreadInfo.GetLink(),
                                                                          threadInfo.ThreadInfo.LastLoadedPostPosition + 1);
                threadInfo.UnreadPosts += newThread.Posts.Count;
                threadInfo.ThreadInfo.LastLoadedPostPosition += newThread.Posts.Count;
            }
            catch {
                // It's fine
            }

            threadInfo.IsLoading = false;
        }

        public void NavigateToThread(ThreadInfoViewModel threadInfoViewModel) {
            ThreadInfo threadInfo = threadInfoViewModel.ThreadInfo;
            ThreadNavigation navigation = ThreadNavigation
                .NavigateToThread(threadInfo.BoardId, threadInfo.Number)
                .ScrollToPostByPosition(threadInfo.LastReadPostPosition + 1)
                .WithHighlighting(threadInfo.LastReadPostPosition + 1);
            Shell.Navigate<ThreadViewModel>(navigation);
        }
    }
}