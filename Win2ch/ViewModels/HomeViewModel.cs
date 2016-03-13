using System.Collections.Generic;
using Core.Common;
using Core.Models;
using Win2ch.Common;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private IShell Shell { get; }
        private IBoard Board { get; }

        public HomeViewModel(IShell shell,
                             IBoard board) {
            Shell = shell;
            Board = board;
        }

        protected override async void OnActivate(object parameter) {
            IList<BoardsCategory> categories = await Board.Operations.BoardsReceiving.ExecuteAsync();
        }
    }
}
