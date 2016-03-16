using System;

namespace Makaba.Services.Url {
    public interface IUrlService {
        Uri GetBoardsListUrl();
        Uri GetFullUrl(string relativeUrl);
        Uri GetBoardUrl(string id, int page = 0);
    }
}