using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common;
using Core.Models;
using Win2ch.Common;
using Win2ch.Common.UI;
using Win2ch.Extensions;
using Win2ch.Services.Storage;

namespace Win2ch.ViewModels {
    public abstract class ThreadsRepositoryViewModelBase : PropertyChangedBase {
        private bool isLoading;

        protected ThreadsRepositoryViewModelBase(IThreadsRepositoryService repositoryService, IBoard board, IShell shell) {
            RepositoryService = repositoryService;
            Board = board;
            Shell = shell;
            Threads = new ObservableCollection<ThreadInfoViewModel>();
        }

        private IBoard Board { get; }
        private IShell Shell { get; }
        private IThreadsRepositoryService RepositoryService { get; }

        public ObservableCollection<ThreadInfoViewModel> Threads { get; }

        public async void Load() {
            if (this.isLoading)
                return;

            this.isLoading = true;
            Threads.Clear();
            Threads.AddRange(RepositoryService.Items.Select(t => new ThreadInfoViewModel(t)));
            foreach (ThreadInfoViewModel thread in Threads) {
                await UpdateThread(thread);
            }

            await RepositoryService.Save();
            this.isLoading = false;
        }

        public async void RefreshAll() {
            if (this.isLoading)
                return;

            this.isLoading = true;
            foreach (ThreadInfoViewModel thread in Threads) {
                await UpdateThread(thread);
            }

            await RepositoryService.Save();
            this.isLoading = false;
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