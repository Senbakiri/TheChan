using System;

namespace Core.Common {
    public enum LinkType {
        None, Unknown, Board, Thread, Post
    }

    public interface IUrlService {
        Uri GetBoardsListUrl();
        Uri GetFullUrl(string relativeUrl);
        Uri GetBoardPageUrl(string id, int page = 0);
        Uri GetFileUrl(string boardId, string path);
        Uri GetThreadUrl(string boardId, long threadNum, int position = 0);
        LinkType DetermineLinkType(string url);
        LinkBase GetLink(string url);
    }
}