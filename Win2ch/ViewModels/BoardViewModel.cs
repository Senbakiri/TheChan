using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Core.Common;
using Win2ch.Common;
using Core.Models;
using Core.Operations;
using Makaba.Operations;

namespace Win2ch.ViewModels {
    public class BoardViewModel : Tab {
        private BoardPage currentPage;
        private int currentPageNumber = -1;

        public BoardViewModel(IShell shell, IBoard board) {
            Shell = shell;
            Board = board;
            Threads = new ObservableCollection<BoardThreadViewModel>();
            LoadBoardOperation = board.Operations.LoadBoard();
            Pages = new BindableCollection<int>();
        }

        private IShell Shell { get; }
        private IBoard Board { get; }
        private ILoadBoardOperation LoadBoardOperation { get; }
        public ObservableCollection<BoardThreadViewModel> Threads { get; }
        public BindableCollection<int> Pages { get; } 

        public BoardPage CurrentPage {
            get { return this.currentPage; }
            private set {
                if (Equals(value, this.currentPage))
                    return;
                this.currentPage = value;
                NotifyOfPropertyChange();
            }
        }

        public int CurrentPageNumber {
            get { return this.currentPageNumber; }
            set {
                if (value == this.currentPageNumber || value < 0)
                    return;
                this.currentPageNumber = value;
                NotifyOfPropertyChange();
                if (value != CurrentPage.Number)
                    ChangePage();
            }
        }

        private async void ChangePage() {
            try {
                await Load(CurrentPageNumber);
            } catch (Exception) {
                Shell.LoadingInfo.Error(GetLocalizationString("Error"));
            }

            IsLoading = false;
        }

        protected override async void OnActivate(object parameter = null) {
            string id;
            var boardInfo = parameter as BriefBoardInfo;
            if (!string.IsNullOrWhiteSpace(boardInfo?.Id))
                id = boardInfo.Id;
            else if (parameter is string)
                id = (string) parameter;
            else
                return;

            DisplayName = boardInfo?.Name ?? $"/{id}/";
            LoadBoardOperation.Id = id;

            try {
                await Load(0);
            } catch (Exception) {
                Shell.LoadingInfo.Error(GetLocalizationString("Error"));
            }

            IsLoading = false;
        }

        private async Task Load(int pageNum) {
            Shell.LoadingInfo.InProgress(GetLocalizationString("Loading"));
            IsLoading = true;
            LoadBoardOperation.Page = pageNum;
            Threads.Clear();
            BoardPage page = await LoadBoardOperation.ExecuteAsync();
            if (Pages.Count == 0)
                Pages.AddRange(page.Pages);
            CurrentPage = page;
            CurrentPageNumber = page.Number;
            DisplayName = CurrentPage.BoardName;
            FillThreads();
            Shell.LoadingInfo.Success(GetLocalizationString("Success"));
        }

        private void FillThreads() {
            foreach (BoardThread thread in CurrentPage.Threads) {
                Threads.Add(new BoardThreadViewModel(thread));
            }
        }

        public void NavigateToThread(ItemClickEventArgs args) {
            var boardThread = (BoardThreadViewModel) args.ClickedItem;
            ThreadNavigation navigation =
                ThreadNavigation.NavigateToThread(CurrentPage.BoardId,
                                                  boardThread.ThreadInfo.Number);
            Shell.Navigate<ThreadViewModel>(navigation);
        }
    }
}