using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common;
using Core.Models;
using Win2ch.Common;
using Win2ch.Extensions;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {

        public HomeViewModel(IShell shell,
                             IBoard board) {
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
            LoadingInfo loadingInfo = Shell.LoadingInfo;
            try {
                loadingInfo.InProgress(GetLocalizationString("ReceivingBoards"));
                IList<BoardsCategory> categories = await Board.Operations.BoardsReceiving.ExecuteAsync();
                Categories.AddRange(categories);
                loadingInfo.Success(GetLocalizationString("BoardsLoaded"));
            } catch (Exception) {
                loadingInfo.Error(GetLocalizationString("BoardsNotLoaded"), true, () => OnActivate());
            }
        }
    }
}
