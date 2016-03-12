using System;

namespace Makaba.Services.Url {
    public class UrlService : IUrlService {
        private const string BaseUrl = "https://2ch.hk/";

        public Uri GetBoardsListUrl() {
            return new Uri(BaseUrl + "makaba/mobile.fcgi?task=get_boards");
        }
    }
}