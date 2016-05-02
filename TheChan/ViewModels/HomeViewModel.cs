using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
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
    internal sealed class HomeViewModel : Tab {
        public HomeViewModel(IShell shell, IBoard board, FavoriteBoardsService favoriteBoardsService) {
            IsCloseable = false;
            DisplayName = GetLocalizationString("Name");
            Shell = shell;
            Board = board;
            FavoriteBoardsService = favoriteBoardsService;
            Categories = new BindableCollection<BoardsCategory>();
        }

        private IShell Shell { get; }
        private IBoard Board { get; }
        private FavoriteBoardsService FavoriteBoardsService { get; }
        public ObservableCollection<BoardsCategory> Categories { get; }
        
        protected override async void OnActivate(object parameter = null) {
           if (Categories.Count == 0)
                await LoadCategories();
        }

        protected override async void OnInitialize() {
            await LoadCategories();
        }

        private async Task LoadCategories() {
            LoadingInfo loadingInfo = Shell.LoadingInfo;
            try {
                loadingInfo.InProgress(GetLocalizationString("ReceivingBoards"));
                Categories.Clear();
                Categories.Add(new BoardsCategory(GetLocalizationString("FavoriteBoards"), FavoriteBoardsService.Items.ToList()));
                IList<BoardsCategory> categories = await Board.LoadBoardsAsync();
                Categories.AddRange(categories);
                loadingInfo.Success(GetLocalizationString("BoardsLoaded"));
            } catch (Exception) {
                loadingInfo.Error(GetLocalizationString("BoardsNotLoaded"), true, () => OnActivate());
            }
        }

        public IList<BoardsCategory> FilterItems(string filter) {
            filter = filter?.ToUpper() ?? "";
            return
                Categories.Select(c => new BoardsCategory(c.Name,
                    c.Boards.Where(b => b.Name.ToUpper().Contains(filter) || b.Id.ToUpper().Contains(filter)).ToList()))
                          .ToList();
        }

        public void NavigateToBoard(BriefBoardInfo briefBoardInfo) {
            Shell.Navigate<BoardViewModel>(briefBoardInfo);
        }

        private void NavigateToBoard(string id) {
            Shell.Navigate<BoardViewModel>(id);
        }

        private void NavigateToThread(string boardId, long num) {
            Shell.Navigate<ThreadViewModel>(ThreadNavigation.NavigateToThread(boardId, num));
        }

        private void NavigateToPost(string boardId, long threadNum, long postNum) {
            Shell.Navigate<ThreadViewModel>(ThreadNavigation.NavigateToThread(boardId, threadNum).ScrollToPost(postNum));
        }

        public async void NavigateByString(string text) {
            LinkType type = Board.UrlService.DetermineLinkType(text);
            LinkBase link = Board.UrlService.GetLink(text);

            switch (type) {
                case LinkType.None:
                    NavigateToBoard(text);
                    break;
                case LinkType.Unknown:
                    await Launcher.LaunchUriAsync(new Uri(text));
                    break;
                case LinkType.Board:
                    NavigateToBoard(((BoardLink) link).BoardId);
                    break;
                case LinkType.Thread:
                    var threadLink = (ThreadLink)link;
                    NavigateToThread(threadLink.BoardId, threadLink.ThreadNumber);
                    break;
                case LinkType.Post:
                    var postLink = (PostLink)link;
                    NavigateToPost(postLink.BoardId, postLink.ThreadNumber, postLink.PostNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
