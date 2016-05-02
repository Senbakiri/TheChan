using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Core.Common;
using Core.Common.Links;
using Core.Models;
using TheChan.Common;
using TheChan.Common.Core;
using TheChan.Common.UI;
using TheChan.Extensions;
using TheChan.Services.Storage;

namespace TheChan.ViewModels {
    public class BoardViewModel : Tab {
        private BoardPage currentPage;
        private int currentPageNumber = -1;
        private bool isInFavorites;

        public BoardViewModel(IShell shell,
                              IBoard board,
                              IAttachmentViewer attachmentViewer,
                              FavoriteBoardsService favoriteBoardsService) {
            Shell = shell;
            Board = board;
            AttachmentViewer = attachmentViewer;
            FavoriteBoardsService = favoriteBoardsService;
            Threads = new ObservableCollection<BoardThreadViewModel>();
            Pages = new BindableCollection<int>();
        }

        private IShell Shell { get; }
        private IBoard Board { get; }
        private IAttachmentViewer AttachmentViewer { get; }
        private FavoriteBoardsService FavoriteBoardsService { get; }
        private BoardLink Link { get; set; }
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

        public bool IsInFavorites {
            get { return this.isInFavorites; }
            private set {
                if (value == this.isInFavorites)
                    return;
                this.isInFavorites = value;
                NotifyOfPropertyChange();
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
            if (!string.IsNullOrWhiteSpace(boardInfo?.Id)) {
                id = boardInfo.Id;
            } else if (parameter is string) {
                id = (string) parameter;
            } else {
                return;
            }

            IsInFavorites = FavoriteBoardsService.Items.Any(b => b.Id.EqualsNc(id));
            DisplayName = boardInfo?.Name ?? $"/{id}/";
            Link = new BoardLink(id);

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
            Threads.Clear();
            BoardPage page = await Board.LoadBoardPageAsync(Link.BoardId, pageNum);

            if (Pages.Count == 0)
                Pages.AddRange(page.Pages);
            CurrentPage = page;
            CurrentPageNumber = page.Number;
            DisplayName = CurrentPage.BoardName;
            FillThreads();
            Shell.LoadingInfo.Success(GetLocalizationString("Success"));
            IsLoading = false;
        }

        public new async void Refresh() {
            if (IsLoading)
                return;
            await Load(CurrentPageNumber);
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

        public async void Favorite() {
            if (CurrentPage == null)
                return;
            
            var favItem = FavoriteBoardsService.Items.FirstOrDefault(b => b.Id.EqualsNc(CurrentPage.BoardId));
            if (favItem != null) {
                FavoriteBoardsService.Items.Remove(favItem);
            } else {
                FavoriteBoardsService.Items.Add(new BriefBoardInfo(
                    0, "", false, false,
                    false, false, false,
                    new List<Icon>(),
                    CurrentPage.BoardId,
                    CurrentPage.BoardName));
            }

            IsInFavorites = favItem == null;
            await FavoriteBoardsService.Save();
        }
    }
}