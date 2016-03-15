using Win2ch.Common;
using Core.Models;

namespace Win2ch.ViewModels {
    public class BoardViewModel : Tab {
        protected override void OnActivate(object parameter = null) {
            var boardPage = parameter as BoardPage;
            if (boardPage == null)
                return;
        }
    }
}