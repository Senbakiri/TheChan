using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Caliburn.Micro;
using Win2ch.Common;
using Core.Models;
using Core.Operations;

namespace Win2ch.ViewModels {
    public class BoardViewModel : Tab {
        private BoardPage currentPage;

        public BoardViewModel(IShell shell, ILoadBoardOperation loadBoardOperation) {
            Shell = shell;
            LoadBoardOperation = loadBoardOperation;
            Threads = new BindableCollection<BoardThreadViewModel>();
        }

        private IShell Shell { get; }
        private ILoadBoardOperation LoadBoardOperation { get; }
        public BindableCollection<BoardThreadViewModel> Threads { get; }

        public BoardPage CurrentPage {
            get { return this.currentPage; }
            private set {
                if (Equals(value, this.currentPage))
                    return;
                this.currentPage = value;
                NotifyOfPropertyChange();
            }
        }

        protected override async void OnActivate(object parameter = null) {
            var boardInfo = parameter as BriefBoardInfo;
            if (string.IsNullOrWhiteSpace(boardInfo?.Id))
                return;

            DisplayName = boardInfo.Name ?? $"/{boardInfo.Id}/";
            LoadBoardOperation.Id = boardInfo.Id;
            LoadBoardOperation.Page = 0;
            try {
                Shell.LoadingInfo.InProgress(GetLocalizationString("Loading"));
                IsLoading = true;
                CurrentPage = await LoadBoardOperation.ExecuteAsync();
                DisplayName = CurrentPage.BoardName;
                FillThreads();
                Shell.LoadingInfo.Success(GetLocalizationString("Success"));
            } catch (Exception) {
                Shell.LoadingInfo.Error(GetLocalizationString("Error"));
            }

            IsLoading = false;
        }

        private void FillThreads() {
            Threads.Clear();
            foreach (BoardThread thread in CurrentPage.Threads) {
                Threads.Add(new BoardThreadViewModel(thread));
            }
        }
    }
}