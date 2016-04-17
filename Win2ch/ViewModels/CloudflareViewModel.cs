using System;
using Core.Common;
using Win2ch.Common;

namespace Win2ch.ViewModels {
    public class CloudflareViewModel : Tab {
        public CloudflareViewModel(IBoard board) {
            Board = board;
            DisplayName = GetLocalizationString("Name");
            PageUri = Board.UrlService.GetFullUrl("/");
        }

        private IBoard Board { get; }
        public Uri PageUri { get; }
    }
}