using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Caliburn.Micro;
using Core.Common;
using Core.Models;
using Core.Operations;
using Makaba.Links;
using Win2ch.Common;
using Win2ch.Extensions;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private string filter;

        public HomeViewModel(IShell shell, IBoard board) {
            IsCloseable = false;
            DisplayName = GetLocalizationString("Name");
            Shell = shell;
            Board = board;
            Categories = new BindableCollection<BoardsCategory>();
        }

        private IShell Shell { get; }
        private IBoard Board { get; }
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
                IList<BoardsCategory> categories = await Board.Operations.LoadBoards().ExecuteAsync();
                Categories.Clear();
                Categories.AddRange(categories);
                loadingInfo.Success(GetLocalizationString("BoardsLoaded"));
            } catch (Exception) {
                loadingInfo.Error(GetLocalizationString("BoardsNotLoaded"), true, () => OnActivate());
            }
        }

        public IList<BoardsCategory> FilterItems(string filter) {
            return
                Categories.Select(c => new BoardsCategory(c.Name,
                    c.Boards.Where(b => b.Name.Contains(filter) || b.Id.Contains(filter)).ToList()))
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
                case LinkType.Post:
                    var threadLink = (ThreadLink)link;
                    NavigateToThread(threadLink.BoardId, threadLink.ThreadNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
