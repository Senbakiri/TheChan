using System;
using System.Linq;
using Core.Common;
using HtmlAgilityPack;
using Makaba.Links;

namespace Makaba.Services.Url {
    public class UrlService : IUrlService {
        private static readonly string[] Domains = new[]
        {"2ch.hk", "2ch.pm", "2ch.re", "2ch.tf", "2ch.wf", "2ch.yt", "2-ch.so"}; 
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

        public Uri GetThreadUrl(string boardId, long threadNum, int position = 0) {
            return new Uri(BaseUri,
                $"/makaba/mobile.fcgi?task=get_thread&board={boardId}&thread={threadNum}&post={position}");
        }

        public Uri GetPostUrl(string boardId, long postNum) {
            return new Uri(BaseUri, $"/makaba/mobile.fcgi?task=get_post&board={boardId}&post={postNum}");
        }

        public LinkType DetermineLinkType(string url) {
            if (url == null)
                return LinkType.None;

            string clearUrl = HtmlEntity.DeEntitize(url);

            Uri uri;
            if (Uri.IsWellFormedUriString(clearUrl, UriKind.Absolute))
                uri = new Uri(clearUrl);
            else if (clearUrl.StartsWith("/") && Uri.IsWellFormedUriString(BaseUrl + clearUrl, UriKind.Absolute))
                uri = new Uri(BaseUri, clearUrl);
            else
                return LinkType.None;

            if (!Domains.Contains(uri.Host))
                return LinkType.Unknown;

            switch (uri.Segments.Length) {
                case 2:
                    return LinkType.Board;
                case 4:
                    long foo;
                    return uri.Fragment.Length > 0 && long.TryParse(uri.Fragment.Trim('#'), out foo)
                        ? LinkType.Post
                        : LinkType.Thread;
                default:
                    return LinkType.Unknown;
            }
        }

        public LinkBase GetLink(string url) {
            if (url == null)
                return null;

            string clearUrl = HtmlEntity.DeEntitize(url);
            LinkType type = DetermineLinkType(clearUrl);
            if (type == LinkType.None || type == LinkType.Unknown)
                return null;

            Uri uri = Uri.IsWellFormedUriString(clearUrl, UriKind.Absolute) ? new Uri(clearUrl) : new Uri(BaseUri, clearUrl);

            string[] segments = uri.Segments;

            string boardId = segments[1].Trim('/');

            if (type == LinkType.Board)
                return new BoardLink(boardId);
            string rawThreadNum = segments[3];
            long threadNum;
            int extensionIndex = rawThreadNum.IndexOf(".html", StringComparison.Ordinal);
            if (extensionIndex != -1)
                rawThreadNum = rawThreadNum.Remove(extensionIndex);
            if (!long.TryParse(rawThreadNum, out threadNum))
                return null;
            string rawPostNum = uri.Fragment.Trim('#');
            long postNum;

            if (type == LinkType.Thread || !long.TryParse(rawPostNum, out postNum))
                return new ThreadLink(boardId, threadNum);

            return type == LinkType.Post ? new PostLink(boardId, threadNum, postNum) : null;
        }
    }
}