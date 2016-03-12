using System.Collections.Generic;
using Makaba.Entities;
using Makaba.Operations;
using Win2ch.Common;

namespace Win2ch.ViewModels {
    internal sealed class HomeViewModel : Tab {
        private IShell Shell { get; }
        private GetBoardsOperation GetBoardsOperation { get; }

        public HomeViewModel(IShell shell,
                             GetBoardsOperation getBoardsOperation) {
            Shell = shell;
            GetBoardsOperation = getBoardsOperation;
        }

        protected override async void OnActivate(object parameter) {
        }
    }
}
