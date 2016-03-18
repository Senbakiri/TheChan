using System;

namespace Makaba.Services.Url {
    public interface IUrlService {
        Uri GetBoardsListUrl();
        Uri GetFullUrl(string relativeUrl);
        Uri GetBoardPageUrl(string id, int page = 0);
        Uri GetFileUrl(string boardId, string path);
    }
}