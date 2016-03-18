using System;

namespace Makaba.Services.Url {
    public class UrlService : IUrlService {
        private const string BaseUrl = "https://2ch.hk";
        private static Uri BaseUri { get; } = new Uri(BaseUrl);
        private const string BoardsListUrl = "/makaba/mobile.fcgi?task=get_boards";

        public Uri GetBoardsListUrl() {
            return new Uri(BaseUrl + BoardsListUrl);
        }

        public Uri GetFullUrl(string relativeUrl) {
            return new Uri(BaseUri, relativeUrl);
        }

        public Uri GetBoardPageUrl(string id, int page = 0) {
            string pageText = page == 0 ? "index" : page.ToString();
            return new Uri(BaseUri, $"/{id}/{pageText}.json");
        }

        public Uri GetFileUrl(string boardId, string path) {
            return new Uri(BaseUri, $"/{boardId}/{path}");
        }
    }
}