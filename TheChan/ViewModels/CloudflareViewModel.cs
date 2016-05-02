using System;
using Core.Common;
using TheChan.Common.Core;

namespace TheChan.ViewModels {
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