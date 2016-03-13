using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        protected override async void OnActivate(object parameter) {
            Shell.LoadingInfo.IsLoading = true;
            Shell.LoadingInfo.LoadingText = GetLocalizationString("ReceivingBoards");
            try {
                IList<BoardsCategory> categories = await Board.Operations.BoardsReceiving.ExecuteAsync();
                Categories.AddRange(categories);
            } catch (Exception) {
                // TODO: Exception handling
            }

            Shell.LoadingInfo.IsLoading = false;
        }
    }
}
