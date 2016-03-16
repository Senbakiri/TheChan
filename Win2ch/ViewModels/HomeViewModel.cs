using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Core.Models;
using Core.Operations;
using Win2ch.Common;
using Win2ch.Extensions;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {

        public HomeViewModel(IShell shell, IHttpOperation<IList<BoardsCategory>> loadBoardsOperation) {
            IsCloseable = false;
            DisplayName = GetLocalizationString("Name");
            Shell = shell;
            LoadBoardsOperation = loadBoardsOperation;
            Categories = new BindableCollection<BoardsCategory>();
        }

        private IShell Shell { get; }
        private IHttpOperation<IList<BoardsCategory>> LoadBoardsOperation { get; }
        public ObservableCollection<BoardsCategory> Categories { get; } 

        protected override async void OnActivate(object parameter = null) {
            LoadingInfo loadingInfo = Shell.LoadingInfo;
            try {
                loadingInfo.InProgress(GetLocalizationString("ReceivingBoards"));
                IList<BoardsCategory> categories = await LoadBoardsOperation.ExecuteAsync();
                Categories.Clear();
                Categories.AddRange(categories);
                loadingInfo.Success(GetLocalizationString("BoardsLoaded"));
            } catch (Exception) {
                loadingInfo.Error(GetLocalizationString("BoardsNotLoaded"), true, () => OnActivate());
            }
        }

        public void NavigateToBoard(BriefBoardInfo briefBoardInfo) {
            Shell.Navigate<BoardViewModel>(briefBoardInfo);
        }
    }
}
