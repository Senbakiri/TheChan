using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.Converters;
using Core.Models;
using Makaba.Entities;
using Makaba.Services.Url;

namespace Makaba.Converters {
    public class BoardConverter : IConverter<BoardPageEntity, BoardPage> {
        public BoardConverter(IUrlService urlService) {
            UrlService = urlService;
        }

        private IUrlService UrlService { get; }

        public BoardPage Convert(BoardPageEntity source) {
            return new BoardPage(
                source.BoardId,
                source.BoardName,
                source.BoardInfo,
                source.CurrentPage,
                source.Pages,
                source.Threads.Select(t => CreateThread(source, t)).ToList());
        }

        private BoardThread CreateThread(BoardPageEntity boardPage, BoardThreadEntity thread) {
            IList<BoardPostEntity> additionalPosts = thread.Posts.Skip(1).ToList();
            int skippedPosts = thread.PostsCount + additionalPosts.Count;
            int skippedPostsWithFiles = thread.FilesCount + additionalPosts.Count(e => e.Files.Count > 0);
            BoardPostEntity post = thread.Posts.First();
            return new BoardThread(
                thread.ThreadNum,
                 new Post(
                    post.Num,
                    post.Parent,
                    post.Subject,
                    post.Name,
                    post.Trip,
                    post.Email,
                    post.Comment,
                    post.IsOp,
                    post.IsBanned,
                    post.IsClosed,
                    post.Sticky != 0,
                    post.Files.Select(f =>
                        new Attachment(
                            f.Name,
                            f.Path,
                            UrlService.GetFileUrl(boardPage.BoardId, f.Path),
                            f.Size,
                            f.Width,
                            f.Height,
                            f.Thumbnail,
                            UrlService.GetFileUrl(boardPage.BoardId, f.Thumbnail),
                            f.ThumbnailWidth,
                            f.ThumbnailHeight,
                            (AttachmentType)f.Type)).ToList(),
                    DateUtils.TimestampToDateTime(post.Timestamp)),
                skippedPosts,
                skippedPostsWithFiles);
        }
    }
}